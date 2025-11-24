using UnityEngine;
using System.Collections.Generic;

public class BoneDataUploader : MonoBehaviour
{
    public static BoneDataUploader Instance;

    public static readonly List<InstanceBinder> Characters = new List<InstanceBinder>();

    // GPU Buffer
    ComputeBuffer boneBuffer;

    // 配置
    public int MaxCharacters = 512; // 最大角色数
    public int MaxBonesPerCharacter = 32; // 每个角色的最大骨骼数

    Matrix4x4[] boneMatrices;

    void Awake()
    {
        Instance = this;

        boneMatrices = new Matrix4x4[MaxCharacters * MaxBonesPerCharacter];
        boneBuffer = new ComputeBuffer(boneMatrices.Length, sizeof(float) * 16);

        Shader.SetGlobalBuffer("_BoneBuffer", boneBuffer);
    }

    public static void RegisterCharacter(InstanceBinder binder)
    {
        Characters.Add(binder);
    }

    void LateUpdate()
    {
        // 将所有角色骨骼塞进一个大数组，GPU 一次性吃完
        foreach (var character in Characters)
        {
            int baseIndex = character.InstanceID * MaxBonesPerCharacter;

            for (int i = 0; i < character.Bones.Length; i++)
            {
                boneMatrices[baseIndex + i] = character.Bones[i].localToWorldMatrix;
            }
        }

        // 一次性上传
        boneBuffer.SetData(boneMatrices);
    }

    void OnDestroy()
    {
        boneBuffer?.Release();
    }
}