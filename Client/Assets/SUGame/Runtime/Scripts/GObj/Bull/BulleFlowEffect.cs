using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BulleFlowEffect : MonoBehaviour
{
    public bool Switch = true;
    public MeshRenderer meshrender;
    public Material mat;
    
    private MaterialPropertyBlock propBlock;
    private int _StretchValueID;
    private int _ElapsedTimeID;
    private int _EmissionID;
    
    private void Awake()
    {

    }

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        meshrender = gameObject.GetComponent<MeshRenderer>();
        mat = meshrender.sharedMaterial;
        // 获取 Shader 变量的 ID（提高性能）
        //bulletFlowEffectID = Shader.PropertyToID("BulletFlowEffect");
        _StretchValueID = Shader.PropertyToID("_StretchValue");
        _ElapsedTimeID = Shader.PropertyToID("_ElapsedTime");
        _EmissionID = Shader.PropertyToID("_Emission");
        SetBulleFlowEffect(transform.localScale.x, Time.time, RandomColor());

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            Debug.Log($"key:C, mat={mat.shader.name}");
            if(!Switch)return;//测试代码，这里应该判断子弹位移id，来触发这个单一子弹效果
            //mat.SetFloat("_FlowSpeed",1);
            SetBulleFlowEffect(transform.localScale.x, Time.time, RandomColor());
        }
    }
    
    void SetBulleFlowEffect(float stretchValue, float elapsedTime, Color emission)
    {
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
