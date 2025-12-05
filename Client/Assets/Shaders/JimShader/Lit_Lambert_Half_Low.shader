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
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };
            
            v2f vert(a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz); // 正确变换到裁剪空间
                o.worldNormal = TransformObjectToWorldNormal(v.normal); // 变换法线到世界空间
                o.worldPos = TransformObjectToWorld(v.vertex.xyz); // 变换顶点位置到世界空间
                o.uv = v.uv;
                return o;
            }
            half4 frag(v2f i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                Light mainLight = GetMainLight(); // 获取主光源信息（颜色、方向）
                half3 lightColor = mainLight.color * mainLight.distanceAttenuation * mainLight.shadowAttenuation;
                float3 worldLightDir = normalize(mainLight.direction);
                // Half-Lambert 漫反射模型
                float ndotL = dot(normalize(i.worldNormal), worldLightDir);
                //float halfLambert = NdotL * 0.5 + 0.5; // 映射到 [0,1]，更柔和
                // 视角方向
                //float3 viewDir = normalize(GetWorldSpaceViewDir(i.worldPos));
                //float3 halfDir = normalize(worldLightDir + viewDir);
                // 高光（Blinn-Phong 模型）
                //half NdotH = saturate(dot(normalize(i.worldNormal), halfDir));
                //half3 specular = lightColor * pow(NdotH, _Gloss);
                
                // 整合颜色
                half3 finalColor = tex * ndotL * lightColor * _Color;// + specular;
                //雾效
                float fogZFactor = ComputeVolumeFog(i.worldPos); // 雾因子
                finalColor = ApplyVolumeFog(finalColor, fogZFactor);
                
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Specular"
}