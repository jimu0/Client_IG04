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
    }

    public List<BodyConfig> bodyConfigs = new List<BodyConfig>();
    public int anchorId = 0;

    public float speed = 1;

    private Vector2 pos1 = new Vector2();
    private Vector2 pos2 = new Vector2();

    private List<Body> bodies;
    private SimTime simTime;
    private readonly Dictionary<int, Transform> viewById = new Dictionary<int, Transform>();
    private readonly Dictionary<int, Vector3> baseScaleById = new Dictionary<int, Vector3>();
    private double speedAccumulator = 0.0;

    void Start()
    {
        simTime = new SimTime(0.0);

        bodies = new List<Body>();
        for (int i = 0; i < bodyConfigs.Count; i++)
        {
            var config = bodyConfigs[i];
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
    }

    void RegisterView(int id, Transform view)
    {
        if (view == null) return;

        viewById[id] = view;
        baseScaleById[id] = view.localScale;
    }

    void SyncViews()
    {
        var activeIds = new HashSet<int>();
        foreach (var body in bodies)
        {
            activeIds.Add(body.id);
            if (viewById.TryGetValue(body.id, out var view))
            {
                SyncView(view, body);
            }
        }

        foreach (var kvp in viewById)
        {
            if (kvp.Value == null) continue;
            bool isActive = activeIds.Contains(kvp.Key);
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
            view.localScale = baseScale * radius;
        }
    }
}
