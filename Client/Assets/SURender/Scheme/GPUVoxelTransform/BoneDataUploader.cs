using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BoneDataUploader : MonoBehaviour
{
    public static BoneDataUploader Instance;

    public static readonly List<InstanceBinder> Characters = new List<InstanceBinder>();

    // GPU Buffer
    ComputeBuffer boneBuffer;

    // 配置
    private const int MaxCharacters = 512; // 最大角色 数
    private const int MaxBonesPerCharacter = 16; // 每个角色的最大骨骼数

    private Matrix4x4[] boneMatrices;
    
    public float updateInterval = 0.0167f; //值为0.0333 约 30 FPS
    private static readonly int BoneBuffer = Shader.PropertyToID("_BoneBuffer");

    private void Awake()
    {
        Instance = this;

        boneMatrices = new Matrix4x4[MaxCharacters * MaxBonesPerCharacter];
        boneBuffer = new ComputeBuffer(boneMatrices.Length, sizeof(float) * 16);

        Shader.SetGlobalBuffer(BoneBuffer, boneBuffer);
    }

    private void Start()
    {
        StartCoroutine(UpdateBoneDataRoutine());
    }

    private IEnumerator UpdateBoneDataRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(updateInterval);
        
        while (true)
        {
            UpdateBoneData();
            yield return wait;
        }
        // ReSharper disable once IteratorNeverReturns
    }

    public static void RegisterCharacter(InstanceBinder binder)
    {
        Characters.Add(binder);
    }
    public static void DestroyCharacter(InstanceBinder binder)
    {
        Characters.Remove(binder);
    }

    // void LateUpdate()
    // {
    //     //UpdateBoneData();
    // }

    private void UpdateBoneData()
    {
        if (Characters == null || Characters.Count == 0) return;

        foreach (InstanceBinder character in Characters)
        {
            if (character == null || character.Bones == null) continue;
            
            int baseIndex = character.InstanceID * MaxBonesPerCharacter;

            for (int i = 0; i < character.Bones.Length; i++)
            {
                Transform bone = character.Bones[i];
                if (bone == null) continue;  // 防止 MissingReference
                //if(bone.localToWorldMatrix==boneMatrices[baseIndex + i])continue;
                boneMatrices[baseIndex + i] = bone.localToWorldMatrix;
            }
        }

        boneBuffer.SetData(boneMatrices);
    }

    public static void OnBindTransArray()
    {
        
        InstanceBinder[] components = FindObjectsOfType<InstanceBinder>(true);
        Debug.Log(components.Length);
        Characters.Clear();
        for (int index = 0; index < components.Length; index++)
        {
            InstanceBinder c = components[index];
            c.InstanceID = index;
            c.OnBindTransArray();
            Characters.Add(c);
        }
    }

    private void OnDestroy()
    {
        boneBuffer?.Release();
    }


}