using UnityEngine;

public interface IWindAffectable
{
    void ApplyWindForce(Vector3 force); // 应用风力
    bool IsAffectedByWind(); // 是否受风力影响
    Vector3 GetWindForceApplicationPoint();
}
