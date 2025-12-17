using UnityEngine;

[ExecuteAlways]
public class SizeConstraint : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] public float weight = 1f;
    public Transform targetPos;
    [System.Serializable]public struct ConstraintSettings
    {
        public FreezeScalingAxes axes;
        public ConstraintSettings(FreezeScalingAxes axes)
        {
            this.axes = axes;
        }
    }
    [System.Serializable]public struct FreezeScalingAxes
    {
        public bool x;
        public bool y;
        public bool z;
        public FreezeScalingAxes(bool x,bool y,bool z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    [SerializeField]
    private ConstraintSettings constraintSettings = new() { axes = new FreezeScalingAxes { z = true } };
    private ConstraintSettings Settings => constraintSettings;
    private readonly VariateWatcher floatWatcher = new();
    void Start()
    {
        if (!targetPos) return;
        UpdateDistance(transform.position, targetPos.position);

    }
    void Update()
    {
        if (!targetPos) return;
        UpdateDistance(transform.position, targetPos.position);
    }
    void UpdateDistance(Vector3 posA,Vector3 posB)
    {
        float dist = Vector3.Distance(posA, posB);
        float value = Mathf.Lerp(0f, dist, weight);
        if (!floatWatcher.FloatCheck(value, out float v)) return;
        Transform tsf = transform;
        Vector3 s = Vector3.one;
        if(Settings.axes.z)s.z = v;
        if(Settings.axes.y)s.y = v;
        if(Settings.axes.x)s.x = v;
        tsf.localScale = s;
    }
}
