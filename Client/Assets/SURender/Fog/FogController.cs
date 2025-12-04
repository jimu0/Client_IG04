using UnityEngine;

public class FogController : MonoBehaviour
{
    public bool enableFogZ;
    public bool enableFogY;
    public Transform fogZPos;
    public Transform fogYPos;
    public Color fogZColor = new Color(0.5f, 0.6f, 0.7f, 1);
    public Color fogYColor = new Color(1f, 1f, 1f, 1);
    private float fogZStart;
    private float fogZRange;
    private float fogYStart;
    private float fogYRange;
    void Update()
    {
        SetGlobalFog();
    }

    void SetGlobalFog()
    {
        if (fogZPos == null)
        {
            enableFogZ = false;
            fogZStart = 0;
            fogZRange = 0;
        }
        else
        {
            fogZStart = fogZPos.position.z;
            fogZRange = Mathf.Max(0.001f, fogZPos.localScale.z);
        }
        Shader.SetGlobalFloat("_FogZ_Enable", enableFogZ ? 1 : 0);
        Shader.SetGlobalColor("_FogZ_Color", fogZColor);
        Shader.SetGlobalFloat("_FogZ_Start", fogZStart);
        Shader.SetGlobalFloat("_FogZ_Range", fogZRange);
        if (fogYPos == null)
        {
            enableFogY = false;
            fogYStart = 0;
            fogYRange = 0;
        }
        else
        {
            fogYStart = fogYPos.position.y;
            fogYRange = Mathf.Max(0.001f, fogYPos.localScale.y);
        }
        Shader.SetGlobalFloat("_FogY_Enable", enableFogY ? 1 : 0);
        Shader.SetGlobalColor("_FogY_Color", fogYColor);
        Shader.SetGlobalFloat("_FogY_Start", fogYStart);
        Shader.SetGlobalFloat("_FogY_Range", fogYRange);
    }
}