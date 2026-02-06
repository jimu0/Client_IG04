using UnityEngine;
using System.Collections.Generic;
using IGC.Engine;
using IGC.Engine.Astrophysics;

public class RelativisticSolarDemo : MonoBehaviour
{
    public Transform blackHoleView;
    public double mass0 = 10000.0;
    public Transform sunView;
    public double mass1 = 10.0;
    public Vector2 velocity1=new Vector2(0,2.2f);
    public Transform earthView;
    public double mass2 = 1.0;
    public Vector2 velocity2=new Vector2(0,1.8f);
    
    public float speed = 1;

    private Vector2 pos1=new Vector2();
    private Vector2 pos2=new Vector2();
    
    
    private List<Body> bodies;
    private SimTime simTime;

    void Start()
    {
        pos1 = new Vector2(sunView.position.x,sunView.position.z);
        pos2= new Vector2(earthView.position.x,earthView.position.z);
        
        simTime = new SimTime(0.0);

        bodies = new List<Body>
        {
            // 锚点：黑洞（绝对参考系）
            new Body(
                id: 0,
                mass: mass0,
                position: Vec2Double.Zero,
                velocity: Vec2Double.Zero
            ),

            // 太阳
            new Body(
                id: 1,
                mass: mass1,
                position: new Vec2Double(pos1.x, pos1.y),
                velocity: new Vec2Double(velocity1.x, velocity1.y)
            ),

            // 地球
            new Body(
                id: 2,
                mass: mass2,
                position: new Vec2Double(pos2.x, pos2.y),
                velocity: new Vec2Double(velocity2.x, velocity2.y)
            )
        };
    }

    void FixedUpdate()
    {
        double dt = Time.fixedDeltaTime*speed;

        // 推进宇宙状态
        WorldDynamics.Step(bodies, ref simTime, dt, anchorId: 0);

        // 映射到 Unity（Unity 只是观察者）
        SyncView(blackHoleView, bodies[0]);
        SyncView(sunView, bodies[1]);
        SyncView(earthView, bodies[2]);
    }

    void SyncView(Transform view, Body body)
    {
        if (view == null) return;

        view.position = new Vector3(
            (float)body.position.x,
            0f,
            (float)body.position.y
        );
    }
}