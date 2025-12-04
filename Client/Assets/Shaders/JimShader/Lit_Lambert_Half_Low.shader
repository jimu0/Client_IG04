Shader "Unlit/Lit_Lambert_Half_Low"
{
    Properties
    {
        _Color ("Diffuse Color", Color) = (0.5, 0.5, 0.5, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        //_Gloss ("Gloss", Range(8.0, 256)) = 20
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
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Assets/Shaders/Common/Fog.hlsl"
            
            half4 _Color;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            //float _Gloss;
            
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
                o.uv = v.uv;
                
                return o;
            }

            // ====== Fragment Shader ======
            half4 frag(v2f i) : SV_Target
            {
                // 纹理采样
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                // 获取主光源信息（颜色、方向）
                Light mainLight = GetMainLight();
                half3 lightColor = mainLight.color * mainLight.distanceAttenuation * mainLight.shadowAttenuation;
                float3 worldLightDir = normalize(mainLight.direction);
                // Half-Lambert 漫反射模型
                float NdotL = dot(normalize(i.worldNormal), worldLightDir);
                float halfLambert = NdotL * 0.5 + 0.5; // 映射到 [0,1]，更柔和
                // 视角方向
                //float3 viewDir = normalize(GetWorldSpaceViewDir(i.worldPos));
                //float3 halfDir = normalize(worldLightDir + viewDir);
                // 高光（Blinn-Phong 模型）
                //half NdotH = saturate(dot(normalize(i.worldNormal), halfDir));
                //half3 specular = lightColor * pow(NdotH, _Gloss);

                float3 worldPos = TransformObjectToWorld(i.worldPos).xyz;
                float4x4 objSice = float4x4(float4(1,0,0,0),float4(0,1,0,0),float4(0,0,1,0),float4(0,0,0,1));
                // 雾因子
                float fogZFactor = ComputeSimpleFogZ(worldPos,objSice);
                
                // 整合
                half3 finalColor = tex * NdotL * lightColor * _Color;// + specular;
                finalColor.rgb = ApplySimpleFogZ(finalColor.rgb, fogZFactor);
                
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Specular"
}