Shader "Unlit/Skybox_Low"
{
    Properties
    {
        _Color ("Sky Color", Color) = (0, 0, 0, 1.0)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Background"
            "Queue" = "Background" 
            "IgnoreProjector" = "True"
            "PreviewType" = "Skybox"
        }
        
        Pass
        {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Shaders/Common/Fog.hlsl"
            
            struct a2v
            {
                float4 positionOS   : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            float4 _Color;

            v2f vert(a2v i)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                //float3 worldPos = TransformObjectToWorld(i.positionOS).xyz+2000;
                float4x4 objSice = float4x4(float4(1,0,0,0),float4(0,1,0,0),float4(0,0,1,0),float4(0,0,0,1));

                // 雾因子
                float fogZFactor = ComputeSimpleFogZ(10000000,objSice);
                float3 col = _Color.rgb;
                col = ApplySimpleFogZ(col, fogZFactor);
                return float4(col, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}