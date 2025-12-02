using System.Collections.Generic;
using UnityEngine;

public class InstanceBinder : MonoBehaviour
{
    //private static int GlobalInstanceCounter = 0;

    public int InstanceID;
    public int SkinID;
    public Transform[] Bones; // 手动或动态收集骨骼

    void Awake()
    {
        //InstanceID = GlobalInstanceCounter++;
        BoneDataUploader.RegisterCharacter(this);
    }
    
    void OnDestroy() 
    {
        BoneDataUploader.DestroyCharacter(this);
    }

    public void OnBindTransArray()
    {
        // true = 包含被禁用的物体
        VoxelMeshRenderer[] components = transform.GetComponentsInChildren<VoxelMeshRenderer>(true);
        Bones = new Transform[components.Length];
        int rangeSkinId = SkinID switch
        {
            -1 => Random.Range(0, 16),
            < -1 => 0,
            >16 => 0,
            _ => SkinID
        };
        for (int index = 0; index < components.Length; index++)
        {
            VoxelMeshRenderer c = components[index];
            if (c.transform.parent != null)
            {
                Bones[index] = c.transform.parent;
                c.iId = InstanceID;
                c.sId = rangeSkinId;
                c.bId = index;
            }
        }
    }
}