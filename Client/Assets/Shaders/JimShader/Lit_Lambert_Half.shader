Shader "Unlit/Lit_Lambert_Half"
{
    Properties
    {
        _Color ("Diffuse Color", Color) = (0.5, 0.5, 0.5, 1)
        //_Diffuse ("Diffuse Multiplier", Color) = (0.5, 0.5, 0.5, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _Specular ("Specular Color", Color) = (0.25, 0.25, 0.25, 1)
        _Gloss ("Gloss", Range(8.0, 256)) = 20
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags
             { 
                 "LightMode" = "UniversalForward"
                 "Queue" = "Geometry"
                 "RenderType" = "Opaque"
             }
            
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 引入 URP 核心库
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"

            // ====== Material Properties ======
            half4 _Color;
            //half4 _Diffuse;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _Specular;
            float _Gloss;

            // ====== Vertex Shader Input ======
            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // ====== Fragment Shader Input ======
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            // ====== Vertex Shader ======
            v2f vert(a2v v)
            {
                v2f o;
                // 正确变换到裁剪空间
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                // 变换法线到世界空间
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                // 变换顶点位置到世界空间
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                // 正确计算纹理坐标（使用 TRANSFORM_TEX 宏）
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            // ====== Fragment Shader ======
            half4 frag(v2f i) : SV_Target
            {
                // 采样纹理
                half3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

                // 获取主光源信息（方向、颜色、强度）
                Light mainLight = GetMainLight();
                float3 worldLightDir = normalize(mainLight.direction);
                float3 lightColor = mainLight.color * mainLight.distanceAttenuation * mainLight.shadowAttenuation;

                // Half-Lambert 漫反射模型
                float NdotL = dot(normalize(i.worldNormal), worldLightDir);
                float halfLambert = NdotL * 0.5 + 0.5; // 映射到 [0,1]，更柔和
                half3 diffuse = lightColor * albedo * halfLambert;

                // 环境光（URP 中没有直接的 UNITY_LIGHTMODEL_AMBIENT，这里用一个简易常量代替）
                // ！如果需要真实环境光，请使用 SH 或 URP 环境探头，这里简化处理
                half3 ambient = half3(0.1, 0.1, 0.1); // 可调，或者后期用 UnityGlobalIllumination()

                // 视角方向
                float3 viewDir = normalize(GetWorldSpaceViewDir(i.worldPos));
                float3 halfDir = normalize(worldLightDir + viewDir);

                // 高光（Blinn-Phong 模型）
                float NdotH = saturate(dot(normalize(i.worldNormal), halfDir));
                half3 specular = lightColor * _Specular.rgb * pow(NdotH, _Gloss);
                
                // 最终颜色：环境光 + 漫反射 + 高光
                half3 finalColor = ambient + diffuse + specular;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Specular"
}