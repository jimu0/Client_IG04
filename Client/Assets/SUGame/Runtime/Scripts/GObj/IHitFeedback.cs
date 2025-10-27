using UnityEngine;

public interface IHitFeedback
{
    // 当该物体被击中时调用
    // hitPoint 是射线命中的点，hitNormal 是命中面的法线，damage 可为伤害值（可选）
    void OnHit(Vector3 hitPoint, Vector3 hitNormal, int damage = 1);
}