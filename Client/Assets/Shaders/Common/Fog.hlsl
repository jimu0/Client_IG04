#ifndef SIMPLE_WORLD_ZY_FOG_INCLUDED
#define SIMPLE_WORLD_ZY_FOG_INCLUDED

float _FogZ_Enable;
float _FogZ_Start;
float _FogZ_Range;
float4 _FogZ_Color;
float _FogY_Enable;
float _FogY_Start;
float _FogY_Range;
float4 _FogY_Color;
/// 获取物体世界缩放
float3 GetObjectScale(float4x4 obj2World)
{
    float3 scale;
    scale.x = length(obj2World[0].xyz);
    scale.y = length(obj2World[1].xyz);
    scale.z = length(obj2World[2].xyz);
    return scale;
}

/// 计算雾因子（基于世界空间 z）
float ComputeSimpleFogZ(float3 worldPos,float4x4 obj2World)
{
    float3 scale = GetObjectScale(obj2World);
    float fogStart = _FogZ_Start * scale.z - _FogZ_Range;
    float fogFactor = saturate((worldPos.z - fogStart) / _FogZ_Range);
    return fogFactor * _FogZ_Enable;
}
/// 对最终颜色加雾
float3 ApplySimpleFogZ(float3 color, float fogFactor)
{
    return lerp(color, _FogZ_Color.rgb, fogFactor);
}
/// 计算雾因子（基于世界空间 z）
float ComputeSimpleFogY(float3 worldPos,float4x4 obj2World)
{
    float3 scale = GetObjectScale(obj2World);
    float fogStart = _FogY_Start * scale.y - _FogY_Range;
    float fogFactor = saturate((worldPos.y - fogStart) / _FogY_Range);
    return fogFactor * _FogY_Enable;
}
/// 对最终颜色加雾
float3 ApplySimpleFogY(float3 color, float fogFactor)
{
    return lerp(color, _FogY_Color.rgb, fogFactor);
}
#endif
