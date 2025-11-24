using UnityEngine;

public class InstanceBinder : MonoBehaviour
{
    //private static int GlobalInstanceCounter = 0;

    public int InstanceID;
    public Transform[] Bones; // 手动或动态收集骨骼

    void Awake()
    {
        //InstanceID = GlobalInstanceCounter++;
        BoneDataUploader.RegisterCharacter(this);
    }
}