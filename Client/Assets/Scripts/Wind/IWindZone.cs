using UnityEngine;

public interface IWindZone
{
    Vector3 GetWindVelocity(); // 获取当前风速和方向
    float GetWindForceMultiplier(); // 获取风力乘数
    float GetWindMaxForce(); // 获取最大风力
    float GetRadius(); // 获取风区半径
    float GetInnerRadiusRatio(); // 获取内圈半径比例
    bool ShouldAffect(GameObject obj); // 判断是否应该影响该物体
}