using UnityEngine;

public class WindAffectableObject : MonoBehaviour, IWindAffectable
{
    [Header("风力响应设置")]
    public bool isAffectedByWind = true;
    public float windForceScale = 1f;
    public bool applyTorque = false;
    public float torqueMultiplier = 0.5f;
    
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("WindAffectableObject 需要 Rigidbody 组件", this);
        }
    }

    #region IWindAffectable 实现
    public void ApplyWindForce(Vector3 force)
    {
        if (rb == null || rb.isKinematic) return;
        
        rb.AddForce(force * windForceScale, ForceMode.Force);
        
        if (applyTorque)
        {
            Vector3 randomTorque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)) * torqueMultiplier * force.magnitude;
            rb.AddTorque(randomTorque, ForceMode.Force);
        }
    }

    public bool IsAffectedByWind() => isAffectedByWind;
    #endregion
    
    public Vector3 GetWindForceApplicationPoint()
    {
        // 默认返回碰撞体中心
        var collider = GetComponent<Collider>();
        return collider != null ? collider.bounds.center : transform.position;
    
        // 或者可以返回特定点，如:
        // return transform.position + transform.up * heightOffset;
    }
}