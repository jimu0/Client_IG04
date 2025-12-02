using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VoxelMeshRenderer : MonoBehaviour
{
    public int iId;
    public int sId;
    public int bId;
    Renderer rend;
    //InstanceBinder binder;
    MaterialPropertyBlock mpb;

    // void Awake()
    // {
    //     rend = GetComponent<Renderer>();
    //     binder = GetComponentInParent<InstanceBinder>();
    //
    //     mpb = new MaterialPropertyBlock();
    // }

    private void Start()
    {
        rend = GetComponent<Renderer>();
        //binder = GetComponentInParent<InstanceBinder>();
        
        mpb = new MaterialPropertyBlock();
        mpb.SetInt("_InstanceID", iId);
        mpb.SetInt("_SkinID", sId);
        mpb.SetInt("_BoneID", bId);
        mpb.SetVector("_MeshOffice",transform.localPosition);
        mpb.SetVector("_MeshSize",transform.localScale);
        mpb.SetColor("_BaseColor", new Color(1,1,1f));
        
        rend.SetPropertyBlock(mpb);
    }
    

    // void LateUpdate()
    // {
    //
    //     mpb.SetInt("_InstanceID", BoneDataUploader.Characters[id].InstanceID);
    //     mpb.SetColor("_BaseColor", new Color(1,1,0.5f));
    //     mpb.SetInt("_BoneID", BoneDataUploader.Characters[id].InstanceID);
    //     rend.SetPropertyBlock(mpb);
    // }
}