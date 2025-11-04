using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BulleFlowEffect : MonoBehaviour
{
    public Vector3 posStart;
    public Vector3 posEnd;
    
    public int id;
    public MeshRenderer meshrender;
    //public Material mat;
    
    private MaterialPropertyBlock propBlock;
    private int _StretchValueID= Shader.PropertyToID("_StretchValue");
    private int _ElapsedTimeID = Shader.PropertyToID("_ElapsedTime");
    private int _EmissionID = Shader.PropertyToID("_Emission");
    
    private void Awake()
    {
        //meshrender = gameObject.GetComponent<MeshRenderer>();
        //mat = meshrender.sharedMaterial;
    }

    void Start()
    {
        // DrawTrajectoryBullet(posStart,posEnd);
        //
        // SetBulleFlowEffect(transform.localScale.x, Time.time, RandomColor());

        
    }
    
    void Update()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            //Debug.Log($"key:C, mat={mat.shader.name}");
            if(id!=1)return;//测试代码，这里应该判断子弹位移id，来触发这个单一子弹效果
            //mat.SetFloat("_FlowSpeed",1);
            DrawTrajectoryBullet(posStart,posEnd);
            
            SetBulleFlowEffect(transform.localScale.x, Time.time, RandomColor());
        }
    }
    
    void DrawTrajectoryBullet(Vector3 posA,Vector3 posB)
    {
        Vector3 dir = (posB - posA).normalized;
        Vector3 right = Camera.main ? Vector3.Cross(dir, (Camera.main.transform.position - posA)).normalized : Vector3.right;
        if (right.sqrMagnitude < 1e-4f) right = Vector3.Cross(dir, Vector3.up);
        transform.SetPositionAndRotation(posA, Quaternion.LookRotation(dir, Vector3.Cross(right, dir)) * Quaternion.Euler(0, -90, 0));
        transform.localScale = new Vector3(Vector3.Distance(posA, posB), 1, 1);
    }
    
    void SetBulleFlowEffect(float stretchValue, float elapsedTime, Color emission)
    {
        propBlock ??= new MaterialPropertyBlock();
        meshrender.GetPropertyBlock(propBlock);  // 获取当前的 PropertyBlock
        // 设置实例化参数
        propBlock.SetFloat(_StretchValueID, stretchValue);
        propBlock.SetFloat(_ElapsedTimeID, elapsedTime);
        propBlock.SetVector(_EmissionID, emission);

        meshrender.SetPropertyBlock(propBlock);  // 应用到当前物体
    }

    Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }
}
