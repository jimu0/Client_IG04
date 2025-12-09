#ifndef SIMPLE_WORLD_FOG_INCLUDED
#define SIMPLE_WORLD_FOG_INCLUDED

float  _FogZ_Enable; // 深度雾开关
float  _FogZ_Start; // 深度雾起始位置
float  _FogZ_Range; // 深度雾衰减区间
float4 _FogZ_Color; // 深度雾颜色
float3 _FogZ_Dir; // 深度雾标方向
float3 _FogZ_Origin; // 深度雾标世界位置

float  _FogY_Enable; // 高度雾开关
float  _FogY_Start; // 高度雾起始位置
float  _FogY_Range; // 高度雾衰减区间
float4 _FogY_Color; // 高度雾颜色
float3 _FogY_Dir; // 高度雾标方向
float3 _FogY_Origin; // 高度雾标世界位置
float4x4 _FogW2LMatrix; // 高度雾矩阵

float ComputeVolumeFogZ(float3 worldPos)
{
    //if (_FogZ_Enable < 0.5) return 0.0;
    float distFromOriginZ = dot(worldPos - _FogZ_Origin, normalize(_FogZ_Dir));
    float fogFactorZ = saturate( (distFromOriginZ - _FogZ_Start) / _FogZ_Range);
    return fogFactorZ * _FogZ_Enable;
}

float3 ApplyVolumeFogZ(float3 color, float fogFactor)
{
    return lerp(color, _FogZ_Color.rgb, fogFactor);
}

float ComputeVolumeFogY(float3 worldPos)
{
    //if (_FogZ_Enable < 0.5) return 0.0;
    float distFromOriginY = dot(worldPos - _FogY_Origin, normalize(_FogY_Dir));
    float fogFactorY = saturate( (distFromOriginY - _FogY_Start) / _FogY_Range);
    return fogFactorY * _FogY_Enable;
}

float4 GetMatrixFogCoord(float3 worldPos)
{
    float4 fogCoord = mul(_FogW2LMatrix, float4(worldPos,1));
    return fogCoord;
}

float3 ComputeVolumeFog(float3 color, float3 worldPos)
{
    //if (_FogZ_Enable < 0.5) return 0.0;
    float distFromOriginY = dot(worldPos - _FogY_Origin, normalize(_FogY_Dir));
    float fogFactorY = saturate( (_FogY_Start - distFromOriginY) / _FogY_Range);
    float fogYFactor = fogFactorY * _FogY_Enable;
    float distFromOriginZ = dot(worldPos - _FogZ_Origin, normalize(_FogZ_Dir));
    float fogFactorZ = saturate( (distFromOriginZ - _FogZ_Start) / _FogZ_Range);
    float fogZFactor = fogFactorZ * _FogZ_Enable;
    float3 finalColor = lerp(color, _FogY_Color.rgb, fogYFactor);
    return lerp(finalColor, _FogZ_Color.rgb, fogZFactor);;
}

#endif