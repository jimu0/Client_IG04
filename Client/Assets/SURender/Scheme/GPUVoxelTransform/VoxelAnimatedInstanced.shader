Shader "Custom/VoxelAnimatedInstanced"
{
    Properties
    {
        //_BaseColor("Color", Color) = (1,1,1,1)
        //_BoneID ("Bone ID", Int) = 0
        _MainTex ("Base Texture (RGBA)", 2D) = "white" {}
    }

    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" 
            "Queue" = "Transparent+100"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }
        

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Shaders/Common/Fog.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            // 支持 GPU Instancing
            #pragma multi_compile_instancing

            // Bone buffer 来自 CPU
            StructuredBuffer<float4x4> _BoneBuffer;
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(int, _InstanceID) // 单位编号
                UNITY_DEFINE_INSTANCED_PROP(int, _SkinID) // 单位皮肤编号
                UNITY_DEFINE_INSTANCED_PROP(int, _BoneID) // 单位每根骨骼的编号
                UNITY_DEFINE_INSTANCED_PROP(float3, _MeshOffice) // 模型局部偏移
                UNITY_DEFINE_INSTANCED_PROP(float3, _MeshSize) // 模型轴拉伸
                //UNITY_DEFINE_INSTANCED_PROP(float2, _MeshUVID) // 模型UVID(x行为骨骼纹理序列,y列为单位纹理列)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor) // 模型颜色
            UNITY_INSTANCING_BUFFER_END(Props)

            static const int MaxBonesPerCharacter = 16;

            struct appdata
            {
                float3 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                int instanceID = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceID);
                int skinID = UNITY_ACCESS_INSTANCED_PROP(Props, _SkinID);
                int boneID = UNITY_ACCESS_INSTANCED_PROP(Props, _BoneID);
                float3 meshOffice = UNITY_ACCESS_INSTANCED_PROP(Props, _MeshOffice);
                float3 meshSize = UNITY_ACCESS_INSTANCED_PROP(Props, _MeshSize);
                //float2 meshUVID = UNITY_ACCESS_INSTANCED_PROP(Props, _MeshUVID);
                // 先进行拉伸和位移
                float3 scaledVertex = v.vertex * meshSize + meshOffice;
                
                // base index = 角色ID * maxBonesPerCharacter
                uint baseIndex = instanceID * MaxBonesPerCharacter;
                // 骨骼矩阵
                float4x4 boneMatrix = _BoneBuffer[baseIndex + boneID];

                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, mul(boneMatrix, float4(scaledVertex, 1)));
                o.normal = normalize(mul((float3x3)boneMatrix, v.normal / meshSize));
                //o.normal = mul((float3x3)boneMatrix, v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz); // 变换顶点位置到世界空间

                
                // 更具meshUVID和boneID来使UV偏移到正确的纹理分配位置，
                float2 texelSize = float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y);
                float2 cellSizeUV = texelSize * 32;   // 一个 32×32 的格子的 UV 范围

                float2 offsetUV = float2(boneID, skinID)*cellSizeUV ;
                

                o.uv = v.uv * cellSizeUV + offsetUV;
                
                o.color = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 outCol = tex * i.color;
                //雾效
                float3 forColor = ComputeVolumeFog(outCol,i.worldPos);
                
                return float4(forColor, 1.0);;
            }

            ENDHLSL
        }
    }
}
