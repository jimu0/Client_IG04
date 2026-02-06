using UnityEngine;
using System.Collections.Generic;
using IGC.Engine;
using IGC.Engine.Astrophysics;

public class RelativisticSolarDemo : MonoBehaviour
{
    [System.Serializable]
    public class BodyConfig
    {
        public int id;
        public Transform view;
        public double mass = 1.0;
        public double radius = 1.0;
        public double angularVelocity = 0.0;
        public Vector2 velocity = Vector2.zero;
        public bool useViewPosition = true;
        public Vector2 position = Vector2.zero;
        public bool showLabel = true;
        public Vector3 labelOffset = new Vector3(0f, 1f, 0f);
        public int labelFontSize = 16;
        public bool showRange = true;
        public int rangeSegments = 64;
        public float rangeWidth = 0.05f;
        public TrajectoryBuffer trajectoryBuffer;
    }

    public List<BodyConfig> bodyConfigs = new List<BodyConfig>();
    public int anchorId = 0;
    public Material rangeMaterial;
    public Color rangeColor = Color.cyan;
    public bool showTrajectories = true;
    public int trajectoryStride = 1;
    public int trajectoryMaxSamples = 1024;
    public float trajectoryWidth = 0.05f;
    public Material trajectoryMaterial;
    public Color trajectoryColor = Color.yellow;

    public float speed = 1;

    private List<Body> bodies;
    private SimTime simTime;
    private readonly Dictionary<int, Transform> viewById = new Dictionary<int, Transform>();
    private readonly Dictionary<int, Vector3> baseScaleById = new Dictionary<int, Vector3>();
    private readonly Dictionary<int, TextMesh> labelById = new Dictionary<int, TextMesh>();
    private readonly Dictionary<int, LineRenderer> rangeById = new Dictionary<int, LineRenderer>();
    private readonly Dictionary<int, LineRenderer> trajectoryById = new Dictionary<int, LineRenderer>();
    private readonly Dictionary<int, BodyConfig> configById = new Dictionary<int, BodyConfig>();
    private double speedAccumulator = 0.0;
    private Material runtimeRangeMaterial;
    private Material runtimeTrajectoryMaterial;
    private readonly HashSet<int> activeBodyIds = new HashSet<int>();

    void Start()
    {
        simTime = new SimTime(0.0);

        bodies = new List<Body>();
        for (int i = 0; i < bodyConfigs.Count; i++)
        {
            var config = bodyConfigs[i];
            configById[config.id] = config;
            Vector2 position2D = config.position;
            if (config.useViewPosition && config.view != null)
            {
                position2D = new Vector2(config.view.position.x, config.view.position.z);
            }

            var body = new Body(
                id: config.id,
                mass: config.mass,
                position: new Vec2Double(position2D.x, position2D.y),
                velocity: new Vec2Double(config.velocity.x, config.velocity.y)
            );
            body.radius = config.radius;
            body.angularVelocity = config.angularVelocity;
            bodies.Add(body);

            RegisterView(config.id, config.view);
            RegisterLabel(config);
            RegisterRange(config);
        }
    }

    void FixedUpdate()
    {
        double fixedDt = Time.fixedDeltaTime;
        speedAccumulator += fixedDt * speed;

        // Step simulation (may merge bodies on collision) with a fixed dt.
        while (speedAccumulator >= fixedDt)
        {
            WorldDynamics.Step(bodies, ref simTime, fixedDt, anchorId: anchorId);
            speedAccumulator -= fixedDt;
        }

        SyncViews();
        //SyncTrajectories();
    }

    void RegisterView(int id, Transform view)
    {
        if (view == null) return;

        viewById[id] = view;
        baseScaleById[id] = view.localScale;
    }

    void SyncViews()
    {
        activeBodyIds.Clear();
        foreach (var body in bodies)
        {
            activeBodyIds.Add(body.id);
            bool isMerged = IsMergedBody(body);
            if (viewById.TryGetValue(body.id, out var view))
            {
                SyncView(view, body);
            }
            if (isMerged)
            {
                CleanupMergedLabel(body.id);
            }
            else
            {
                SyncLabel(body);
            }
            SyncRange(body);
        }

        foreach (var kvp in viewById)
        {
            if (kvp.Value == null) continue;
            bool isActive = activeBodyIds.Contains(kvp.Key);
            if (kvp.Value.gameObject.activeSelf != isActive)
            {
                kvp.Value.gameObject.SetActive(isActive);
            }
        }

        foreach (var kvp in labelById)
        {
            if (kvp.Value == null) continue;
            bool isActive = activeBodyIds.Contains(kvp.Key);
            if (kvp.Value.gameObject.activeSelf != isActive)
            {
                if (!isActive)
                {
                    Destroy(kvp.Value.gameObject);
                }
                else
                {
                    kvp.Value.gameObject.SetActive(true);
                }
            }
        }

        foreach (var kvp in rangeById)
        {
            if (kvp.Value == null) continue;
            bool isActive = activeBodyIds.Contains(kvp.Key);
            if (kvp.Value.gameObject.activeSelf != isActive)
            {
                kvp.Value.gameObject.SetActive(isActive);
            }
        }
    }

    void SyncView(Transform view, Body body)
    {
        if (view == null) return;

        view.position = new Vector3(
            (float)body.position.x,
            0f,
            (float)body.position.y
        );
        float yawDegrees = (float)(body.rotation * Mathf.Rad2Deg);
        view.rotation = Quaternion.Euler(0f, -yawDegrees, 0f);

        if (baseScaleById.TryGetValue(body.id, out var baseScale))
        {
            float radius = Mathf.Max(0.0f, (float)body.radius);
            view.localScale = baseScale * (radius * 2);
        }
    }

    void RegisterLabel(BodyConfig config)
    {
        if (!config.showLabel) return;

        var labelObject = new GameObject("BodyLabel_" + config.id);
        labelObject.transform.SetParent(transform, false);
        var textMesh = labelObject.AddComponent<TextMesh>();
        textMesh.fontSize = config.labelFontSize;
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.LowerLeft;
        labelById[config.id] = textMesh;
    }

    void RegisterRange(BodyConfig config)
    {
        if (!config.showRange) return;

        var rangeObject = new GameObject("BodyRange_" + config.id);
        rangeObject.transform.SetParent(transform, false);
        var line = rangeObject.AddComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = true;
        line.widthMultiplier = config.rangeWidth;
        line.positionCount = Mathf.Max(3, config.rangeSegments);
        line.material = GetRangeMaterial();
        line.startColor = rangeColor;
        line.endColor = rangeColor;
        rangeById[config.id] = line;
    }

    void SyncLabel(Body body)
    {
        if (!labelById.TryGetValue(body.id, out var label) || label == null) return;

        if (configById.TryGetValue(body.id, out var config))
        {
            label.fontSize = config.labelFontSize;
        }

        var position = new Vector3((float)body.position.x, 0f, (float)body.position.y);
        var offset = Vector3.up;
        if (configById.TryGetValue(body.id, out var cfg))
        {
            offset = cfg.labelOffset;
        }
        label.transform.position = position + offset;

        var velocity = body.velocity;
        label.text = string.Format(
            "id:{0}\nm:{1:0.###}\nv:({2:0.###},{3:0.###})",
            body.id,
            body.mass,
            velocity.x,
            velocity.y
        );

        label.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    void SyncRange(Body body)
    {
        if (!rangeById.TryGetValue(body.id, out var line) || line == null) return;

        int segments = line.positionCount;
        float radius = Mathf.Max(0.0f, (float)body.radius);
        if (radius <= 0.0f)
        {
            line.enabled = false;
            return;
        }
        line.enabled = true;

        var center = new Vector3((float)body.position.x, 0f, (float)body.position.y);
        for (int i = 0; i < segments; i++)
        {
            float angle = (Mathf.PI * 2f * i) / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            line.SetPosition(i, center + new Vector3(x, 0f, z));
        }
    }

    void SyncTrajectories()
    {
        if (!showTrajectories) return;

        int stride = Mathf.Max(1, trajectoryStride);
        int maxSamples = Mathf.Max(2, trajectoryMaxSamples);

        for (int i = 0; i < bodies.Count; i++)
        {
            var body = bodies[i];
            if (!activeBodyIds.Contains(body.id))
            {
                if (trajectoryById.TryGetValue(body.id, out var inactiveLine) && inactiveLine != null)
                {
                    inactiveLine.gameObject.SetActive(false);
                }
                continue;
            }

            var buffer = GetTrajectoryBuffer(body);
            if (buffer == null)
            {
                if (trajectoryById.TryGetValue(body.id, out var missingLine) && missingLine != null)
                {
                    missingLine.gameObject.SetActive(false);
                }
                continue;
            }

            var points = new List<Vector3>();
            int index = 0;
            foreach (var sample in buffer.Samples)
            {
                if ((index++ % stride) != 0) continue;
                points.Add(ToWorldPosition(sample.position));
                if (points.Count >= maxSamples)
                {
                    break;
                }
            }

            var line = GetTrajectoryLine(body.id);
            if (points.Count < 2)
            {
                line.gameObject.SetActive(false);
                continue;
            }

            line.gameObject.SetActive(true);
            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());
        }
    }

    LineRenderer GetTrajectoryLine(int id)
    {
        if (trajectoryById.TryGetValue(id, out var existing) && existing != null) return existing;

        var lineObject = new GameObject("BodyTrajectory_" + id);
        lineObject.transform.SetParent(transform, false);
        var line = lineObject.AddComponent<LineRenderer>();
        line.loop = false;
        line.useWorldSpace = true;
        line.widthMultiplier = trajectoryWidth;
        line.material = GetTrajectoryMaterial();
        line.startColor = trajectoryColor;
        line.endColor = trajectoryColor;
        trajectoryById[id] = line;
        return line;
    }

    Vector3 ToWorldPosition(Vec2 position)
    {
        return new Vector3(position.x, 0f, position.y);
    }

    TrajectoryBuffer GetTrajectoryBuffer(Body body)
    {
        var type = body.GetType();
        var property = type.GetProperty("trajectoryBuffer")
            ?? type.GetProperty("TrajectoryBuffer")
            ?? type.GetProperty("trajectory")
            ?? type.GetProperty("Trajectory")
            ?? type.GetProperty("history")
            ?? type.GetProperty("History");
        if (property != null)
        {
            return property.GetValue(body, null) as TrajectoryBuffer;
        }

        var field = type.GetField("trajectoryBuffer")
            ?? type.GetField("TrajectoryBuffer")
            ?? type.GetField("trajectory")
            ?? type.GetField("Trajectory")
            ?? type.GetField("history")
            ?? type.GetField("History");
        if (field != null)
        {
            return field.GetValue(body) as TrajectoryBuffer;
        }

        if (configById.TryGetValue(body.id, out var config))
        {
            return config.trajectoryBuffer;
        }

        return null;
    }

    bool IsMergedBody(Body body)
    {
        return body.parentId >= 0 && body.parentId != body.id;
    }

    void CleanupMergedLabel(int id)
    {
        if (labelById.TryGetValue(id, out var label) && label != null)
        {
            Destroy(label.gameObject);
            labelById.Remove(id);
        }
    }

    Material GetRangeMaterial()
    {
        if (rangeMaterial != null) return rangeMaterial;
        if (runtimeRangeMaterial == null)
        {
            runtimeRangeMaterial = new Material(Shader.Find("Sprites/Default"));
        }
        return runtimeRangeMaterial;
    }

    Material GetTrajectoryMaterial()
    {
        if (trajectoryMaterial != null) return trajectoryMaterial;
        if (runtimeTrajectoryMaterial == null)
        {
            runtimeTrajectoryMaterial = new Material(Shader.Find("Sprites/Default"));
        }
        return runtimeTrajectoryMaterial;
    }
}
