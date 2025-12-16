Shader "TDUShader/BulletFlowEffect"
{
    Properties
    {
        _MainTex ("Base Texture (RGBA)", 2D) = "white" {}
        //_Emission ("Emission Color", Color) = (1, 1, 1, 1)
        _FlowSpeed("Flow Speed",float) = 4
        _Light("Light",float) = 1
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            //"RenderType"="Opaque"
        
            "RenderType" = "TransparentCutout"
            "Queue" = "Transparent+100"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }
        //LOD 100
        
        //ZWrite Off
        ZTest LEqual
        //Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "BulletFlowPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag
            
            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Shaders/Common/Fog.hlsl"
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            //float _StretchValue;
            //float _ElapsedTime;
            float _FlowSpeed;
            
            //half4 _Emission;
            float _Light;
            float _Cutoff;
            
            // Instancing 支持
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _StretchValue)
                UNITY_DEFINE_INSTANCED_PROP(float, _ElapsedTime)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Emission) 
            UNITY_INSTANCING_BUFFER_END(Props)

            struct a2v
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(a2v i)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_TRANSFER_INSTANCE_ID(i, o);

                float3 positionWS = TransformObjectToWorld(i.positionOS.xyz);
                o.positionCS = TransformWorldToHClip(positionWS);

                o.worldPos = TransformObjectToWorld(i.positionOS.xyz);
                
                float stretchValue = UNITY_ACCESS_INSTANCED_PROP(Props, _StretchValue);
                float elapsedTime = UNITY_ACCESS_INSTANCED_PROP(Props, _ElapsedTime);

                
                float relativeTime = _Time.y - elapsedTime;//重置运动
                //常量0.0625为纹理宽度16的归一化值，根据具体子弹需要的纹理调整
                o.uv = i.uv * float2(0.0625 * stretchValue,1) + float2(1 + relativeTime * -_FlowSpeed,0);
                
                //o.uv=i.uv*float2(0.5,1.0);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //  经典伪随机，用UV 生成伪随机数（0~1）
                //float randomValue = frac(sin(dot(i.uv, float2(12.9898, 78.233))) * 43758.5453);
                UNITY_SETUP_INSTANCE_ID(i);
                float4 emission = UNITY_ACCESS_INSTANCED_PROP(Props, _Emission);

                float3 forColor = ComputeVolumeFog(emission,i.worldPos);//雾效

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 color = tex * float4(forColor, 1.0) * _Light;
                clip(tex.a - _Cutoff);

                
                color.a = tex.a;

                
                
                
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}