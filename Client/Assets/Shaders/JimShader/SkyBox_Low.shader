Shader "Unlit/Skybox_Low"
{
    Properties
    {
        _Color ("Sky Color", Color) = (0, 0, 0, 1.0)
        [Toggle] _FogEnable("Fog", Float) = 1
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

            struct a2v{float4 positionOS : POSITION;};
            struct v2f{float4 positionCS : SV_POSITION;};
            float4 _Color;
            float _FogEnable;
            
            v2f vert(a2v i)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }
            float4 frag(v2f i) : SV_Target
            {
                float3 col = lerp(_Color.rgb, _FogZ_Color.rgb, _FogZ_Enable * _FogEnable); // 全雾覆盖
                
                return float4(col, 1.0);
            }
            
            ENDHLSL
        }
    }
    Fallback Off
}
