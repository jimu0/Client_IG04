using UnityEngine;
using System.Collections.Generic;
using IGC.Engine;
using IGC.Engine.Astrophysics;

public class RelativisticSolarDemo : MonoBehaviour
{
    public Transform blackHoleView;
    public double mass0 = 10000.0;
    public double radius0 = 3.0;
    public double angularVelocity0 = 0.0;
    public Transform sunView;
    public double mass1 = 10.0;
    public double radius1 = 1.0;
    public double angularVelocity1 = 0.0;
    public Vector2 velocity1 = new Vector2(0, 2.2f);
    public Transform earthView;
    public double mass2 = 1.0;
    public double radius2 = 0.5;
    public double angularVelocity2 = 0.0;
    public Vector2 velocity2 = new Vector2(0, 1.8f);

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
        pos1 = new Vector2(sunView.position.x, sunView.position.z);
        pos2 = new Vector2(earthView.position.x, earthView.position.z);

        simTime = new SimTime(0.0);

        bodies = new List<Body>();

        var blackHole = new Body(
            id: 0,
            mass: mass0,
            position: Vec2Double.Zero,
            velocity: Vec2Double.Zero
        );
        blackHole.radius = radius0;
        blackHole.angularVelocity = angularVelocity0;
        bodies.Add(blackHole);

        var sun = new Body(
            id: 1,
            mass: mass1,
            position: new Vec2Double(pos1.x, pos1.y),
            velocity: new Vec2Double(velocity1.x, velocity1.y)
        );
        sun.radius = radius1;
        sun.angularVelocity = angularVelocity1;
        bodies.Add(sun);

        var earth = new Body(
            id: 2,
            mass: mass2,
            position: new Vec2Double(pos2.x, pos2.y),
            velocity: new Vec2Double(velocity2.x, velocity2.y)
        );
        earth.radius = radius2;
        earth.angularVelocity = angularVelocity2;
        bodies.Add(earth);

        RegisterView(0, blackHoleView);
        RegisterView(1, sunView);
        RegisterView(2, earthView);
    }

    void FixedUpdate()
    {
        double fixedDt = Time.fixedDeltaTime;
        speedAccumulator += fixedDt * speed;

        // Step simulation (may merge bodies on collision) with a fixed dt.
        while (speedAccumulator >= fixedDt)
        {
            WorldDynamics.Step(bodies, ref simTime, fixedDt, anchorId: 0);
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
        view.rotation = Quaternion.Euler(0f, yawDegrees, 0f);

        if (baseScaleById.TryGetValue(body.id, out var baseScale))
        {
            float radius = Mathf.Max(0.0f, (float)body.radius);
            view.localScale = baseScale * radius;
        }
    }
}
