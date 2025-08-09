using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WindSphere : MonoBehaviour, IWindZone
{
    [Header("风力设置")]
    public float forceMultiplier = 10f;
    public float maxForce = 50f;
    public AnimationCurve speedToForceCurve;
    public float innerRadiusRatio = 0.7f;
    
    [Header("过滤设置")]
    public bool affectOnlySpecificLayer = false;
    public LayerMask affectedLayers;

    private SphereCollider sphereCollider;
    private Vector3 previousPosition;
    private Vector3 currentVelocity;
    //Vector3 otherPosition = windAffectable.GetWindForceApplicationPoint();
    
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        previousPosition = transform.position;
    }

    private void Update()
    {
        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    #region IWindZone 实现
    public Vector3 GetWindVelocity() => currentVelocity;
    public float GetWindForceMultiplier() => forceMultiplier;
    public float GetWindMaxForce() => maxForce;
    public float GetRadius() => sphereCollider.radius * transform.lossyScale.x;
    public float GetInnerRadiusRatio() => innerRadiusRatio;
    
    public bool ShouldAffect(GameObject obj)
    {
        if (!affectOnlySpecificLayer) return true;
        return ((1 << obj.layer) & affectedLayers) != 0;
    }
    #endregion

    private void OnTriggerStay(Collider other)
    {
        if (!ShouldAffect(other.gameObject)) return;
        
        IWindAffectable windAffectable = other.GetComponent<IWindAffectable>();
        if (windAffectable == null || !windAffectable.IsAffectedByWind()) return;
        
        // 计算风力
        Vector3 windVelocity = GetWindVelocity();
        float speed = windVelocity.magnitude;
        float forceMagnitude = speedToForceCurve.Evaluate(speed) * GetWindForceMultiplier();
        forceMagnitude = Mathf.Min(forceMagnitude, GetWindMaxForce());
        
        if (forceMagnitude > 0.01f && speed > 0.1f)
        {
            // 获取物体上应用力的位置
            Vector3 otherPosition = windAffectable.GetWindForceApplicationPoint();
            
            // 计算位置衰减
            float distance = Vector3.Distance(transform.position, otherPosition);
            float normalizedDistance = Mathf.Clamp01(
                (distance - GetRadius() * GetInnerRadiusRatio()) / 
                (GetRadius() - GetRadius() * GetInnerRadiusRatio()));
            float positionAttenuation = 1f - normalizedDistance;
            
            Vector3 force = windVelocity.normalized * forceMagnitude * positionAttenuation;
            windAffectable.ApplyWindForce(force);
        }
    }

    private void OnDrawGizmos()
    {
        if (sphereCollider == null) sphereCollider = GetComponent<SphereCollider>();
        
        // 绘制外圈
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetRadius());
        
        // 绘制内圈
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, GetRadius() * innerRadiusRatio);
        
        // 绘制风向
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, currentVelocity.normalized * GetRadius());
    }
}