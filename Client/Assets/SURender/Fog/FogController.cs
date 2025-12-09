using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class FogController : MonoBehaviour
{
    //ForZ
    public bool enableFogZ;
    public GameObject fogZVolume;
    public Color fogZColor = new Color(0.5f, 0.6f, 0.7f, 1);
    public float fogZStart;// 雾的起始位置（沿FogDir）
    public float fogZRange;// 雾的衰减区间
    public ParentConstraint parentConstraint;
    //ForZ缓存
    bool lastEnableFogZ;
    Color lastFogZColor;
    float lastFogZStart;
    float lastFogZRange;
    Vector3 lastFogZOrigin;
    Vector3 lastFogZDir;
    //ForY
    public bool enableFogY;
    public GameObject fogYVolume;
    public Color fogYColor = new Color(0.5f, 0.6f, 0.7f, 1);
    public float fogYStart;// 雾的起始位置（沿FogDir）
    public float fogYRange;// 雾的衰减区间
    //ForY缓存
    bool lastEnableFogY;
    Color lastFogYColor;
    float lastFogYStart;
    float lastFogYRange;
    Vector3 lastFogYOrigin;
    Vector3 lastFogYDir;
    Transform lastFogW2LMatrix;
    
    void Start()
    {
        SetGlobalFog();
    }

    void Update()
    {
        SetGlobalFog();
    }

    void SetGlobalFog()
    {
        if (fogYVolume != null)
        {
            UpdateFogYEnable();
            UpdateFogYColor();
            UpdateFogYStart();
            UpdateFogYRange();
            UpdateFogYPos();
            UpdateFogYRot();
            //UpdateFogW2LMatrix();
        }
        else
        {
            enableFogY = false;
        }

        if (fogZVolume != null)
        {
            fogZVolume.transform.SetPositionAndRotation(Camera.main.transform.position,quaternion.identity);

            UpdateFogZEnable();
            UpdateFogZColor();
            UpdateFogZStart();
            UpdateFogZRange();
            UpdateFogZPos();
            UpdateFogZRot();

            // if (parentConstraint!=null&&parentConstraint.GetSource(0).sourceTransform)
            // {
            //
            // }
            // else
            // {
            //     fogZVolume.transform.SetPositionAndRotation(Camera.main.transform.position,quaternion.identity);
            //     // fogZVolume.GetComponent<ParentConstraint>().SetSources(new List<ConstraintSource>());
            //     // List<ConstraintSource> csList = new();
            //     // ConstraintSource cs = new();
            //     // cs.weight = 1;
            //     // cs.sourceTransform = Camera.main.transform;
            //     // csList.Add(cs);
            //     // parentConstraint.SetSources(csList);
            // }

        }
        else
        {
            enableFogZ = false;
        }




    }

    void UpdateFogZEnable()
    {
        if (enableFogZ == lastEnableFogZ) return;
        Shader.SetGlobalFloat("_FogZ_Enable", enableFogZ ? 1.0f : 0f);
        lastEnableFogZ = enableFogZ;
    }
    void UpdateFogZColor()
    {
        if (fogZColor == lastFogZColor) return;
        Shader.SetGlobalVector("_FogZ_Color", fogZColor);
        lastFogZColor = fogZColor;
    }
    void UpdateFogZStart()
    {
        if (Math.Abs(fogZStart - lastFogZStart) < 0.001f) return;
        Shader.SetGlobalFloat("_FogZ_Start", fogZStart);
        lastFogZStart = fogZStart;
    }
    void UpdateFogZRange()
    {
        if (Math.Abs(fogZRange - lastFogZRange) < 0.001f) return;
        Shader.SetGlobalFloat("_FogZ_Range", fogZRange);
        lastFogZRange = fogZRange;
    }
    void UpdateFogZPos()
    {
        Vector3 pos = fogZVolume.transform.position;
        if (pos == lastFogZOrigin) return;
        Shader.SetGlobalVector("_FogZ_Origin", pos);
        lastFogZOrigin = pos;
    }
    void UpdateFogZRot()
    {
        Vector3 forward = fogZVolume.transform.forward;
        if (forward == lastFogZDir) return;
        Shader.SetGlobalVector("_FogZ_Dir", forward);
        lastFogZDir = forward;
    }
    
    void UpdateFogYEnable()
    {
        if (enableFogY == lastEnableFogY) return;
        Shader.SetGlobalFloat("_FogY_Enable", enableFogY ? 1.0f : 0f);
        lastEnableFogY = enableFogY;
    }
    void UpdateFogYColor()
    {
        if (fogYColor == lastFogYColor) return;
        Shader.SetGlobalVector("_FogY_Color", fogYColor);
        lastFogYColor = fogYColor;
    }
    void UpdateFogYStart()
    {
        if (Math.Abs(fogYStart - lastFogYStart) < 0.001f) return;
        Shader.SetGlobalFloat("_FogY_Start", fogYStart);
        lastFogYStart = fogYStart;
    }
    void UpdateFogYRange()
    {
        if (Math.Abs(fogYRange - lastFogYRange) < 0.001f) return;
        Shader.SetGlobalFloat("_FogY_Range", fogYRange);
        lastFogYRange = fogYRange;
    }
    void UpdateFogYPos()
    {
        Vector3 pos = fogYVolume.transform.position;
        if (pos == lastFogYOrigin) return;
        Shader.SetGlobalVector("_FogY_Origin", pos);
        lastFogYOrigin = pos;
    }
    void UpdateFogYRot()
    {
        Vector3 up = -fogYVolume.transform.up;
        if (up == lastFogYDir) return;
        Shader.SetGlobalVector("_FogY_Dir", up);
        lastFogYDir = up;
    }

    void UpdateFogW2LMatrix()
    {
        Transform pos = fogYVolume.transform;
        if(pos == lastFogW2LMatrix) return;
        Shader.SetGlobalMatrix("_FogW2LMatrix",pos.worldToLocalMatrix);
        lastFogW2LMatrix = pos;
    }

}