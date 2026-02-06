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
    }

    public List<BodyConfig> bodyConfigs = new List<BodyConfig>();
    public int anchorId = 0;

    [Range(0f, 100f)]public float speed = 16f;

    private List<Body> bodies;
    private SimTime simTime;
    private readonly Dictionary<int, Transform> viewById = new Dictionary<int, Transform>();
    private readonly Dictionary<int, Vector3> baseScaleById = new Dictionary<int, Vector3>();
    private readonly Dictionary<int, TextMesh> labelById = new Dictionary<int, TextMesh>();
    private readonly Dictionary<int, BodyConfig> configById = new Dictionary<int, BodyConfig>();
    private double speedAccumulator = 0.0;
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
                position2D = new Vector2(config.view.position.x, config.view.position.y);
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
        }
    }

    void FixedUpdate()
    {
        double fixedDt = 0.001f * speed;
        speedAccumulator += fixedDt * speed;
        
        if (!WTime.Tick()) return;
        int steps = WTime.Advance();
        for (int i = 0; i < steps; i++)
        {
            WorldDynamics.Step(bodies, ref simTime, fixedDt, anchorId: anchorId);
            // while (speedAccumulator >= fixedDt)
            // {
            //     WorldDynamics.Step(bodies, ref simTime, fixedDt, anchorId: anchorId);
            //     speedAccumulator -= fixedDt;
            // }
        }


        SyncViews();
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

    }

    void SyncView(Transform view, Body body)
    {
        if (view == null) return;

        view.position = new Vector3(
            (float)body.position.x,
            (float)body.position.y,
            0f
        );
        float yawDegrees = (float)(body.rotation * Mathf.Rad2Deg);
        view.rotation = Quaternion.Euler(0f, 0f, -yawDegrees);

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


    void SyncLabel(Body body)
    {
        if (!labelById.TryGetValue(body.id, out var label) || label == null) return;

        if (configById.TryGetValue(body.id, out var config))
        {
            label.fontSize = config.labelFontSize;
        }

        var position = new Vector3((float)body.position.x, (float)body.position.y, 0f);
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

        label.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    Vector3 ToWorldPosition(Vec2 position)
    {
        return new Vector3(position.x, position.y, 0f);
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

}
