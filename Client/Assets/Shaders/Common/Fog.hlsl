#ifndef SIMPLE_WORLD_FOG_INCLUDED
#define SIMPLE_WORLD_FOG_INCLUDED

float  _FogZ_Enable; // 雾开关
float  _FogZ_Start; // 雾起始位置
float  _FogZ_Range; // 雾衰减区间
float4 _FogZ_Color; // 雾颜色
float3 _FogDir; // 雾标方向
float3 _FogOrigin; // 雾标世界位置

float ComputeVolumeFog(float3 worldPos)
{
    if (_FogZ_Enable < 0.5) return 0.0;
    float distFromOrigin = dot(worldPos - _FogOrigin, normalize(_FogDir));
    float fogFactor = saturate( (distFromOrigin - _FogZ_Start) / _FogZ_Range);
    return fogFactor;
}

float3 ApplyVolumeFog(float3 color, float fogFactor)
{
    return lerp(color, _FogZ_Color.rgb, fogFactor);
}
#endif
