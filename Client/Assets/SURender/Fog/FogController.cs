using System;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public bool enableFogZ;
    public GameObject fogZVolume;
    public Color fogZColor = new Color(0.5f, 0.6f, 0.7f, 1);
    public float fogZStart;// 雾的起始位置（沿FogDir）
    public float fogZRange;// 雾的衰减区间

    //缓存
    Color lastFogColor;
    float lastFogStart;
    float lastFogRange;
    Vector3 lastFogOrigin;
    Vector3 lastFogDir;

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
        if (fogZVolume == null) enableFogZ = false;
        Shader.SetGlobalFloat("_FogZ_Enable", enableFogZ ? 1.0f : 0f);
        Shader.SetGlobalColor("_FogZ_Color", fogZColor);
        Shader.SetGlobalFloat("_FogZ_Start", fogZStart);
        Shader.SetGlobalFloat("_FogZ_Range", fogZRange);
        Shader.SetGlobalVector("_FogOrigin", fogZVolume.transform.position);
        Shader.SetGlobalVector("_FogDir", fogZVolume.transform.forward);

        
    }

    void UpdateFogColor()
    {
        if (fogZColor == lastFogColor) return;
        Shader.SetGlobalVector("_FogDir", fogZColor);
        lastFogColor = fogZColor;
    }
    void UpdateFogStart()
    {
        if (Math.Abs(fogZStart - lastFogStart) < 0.001f) return;
        Shader.SetGlobalFloat("_FogZ_Start", fogZStart);
        lastFogStart = fogZStart;
    }
    void UpdateFogRange()
    {
        if (Math.Abs(fogZRange - lastFogRange) < 0.001f) return;
        Shader.SetGlobalFloat("_FogZ_Range", fogZRange);
        lastFogRange = fogZRange;
    }
    void UpdateFogZPos()
    {
        Vector3 pos = fogZVolume.transform.position;
        if (pos == lastFogOrigin) return;
        Shader.SetGlobalVector("_FogOrigin", pos);
        lastFogOrigin = pos;
    }
    void UpdateFogZRot()
    {
        Vector3 forward = fogZVolume.transform.forward;
        if (forward == lastFogDir) return;
        Shader.SetGlobalVector("_FogDir", forward);
        lastFogDir = forward;
    }

}