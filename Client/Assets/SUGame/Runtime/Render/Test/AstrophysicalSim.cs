using System;
using System.Collections;
using System.Collections.Generic;
using IGC.Engine;
using IGC.Engine.Astrophysics;
using UnityEngine;

public class AstrophysicalSim : MonoBehaviour
{
    [System.Serializable]
    public class BodyCfg
    {
        public Transform objTsf;
        //public bool useViewPosition = true;
        public int id;
        public double mass;
        [HideInInspector]public double radius;
        //public double positionX;
        //public double positionY;
        public double velocityX;
        public double velocityY;
        //private double angularVelocity;
    }
    
    public List<BodyCfg> bodyCfgs = new();

    
    public double G = 20;//引力常数
    public double k_contact = 0.005f;//钢性因子()
    public double k_absorb = 30f;//吞噬因子()
    [Range(0.008333f,0.166667f)]
    public double fixedDt = WTime.fixedDt;//时间步长
    [Range(0.1f, 5f)]
    public double timeScale = 1f;//时间速率

    private void Start()
    {
        
        BuildBodys();
    }

    // private void Update()
    // {
    //
    // }

    void FixedUpdate()
    {
        //double fixedDt = 0.001f * speed;
        //speedAccumulator += fixedDt * speed;
        //Debug.Log(AstrophysicalSys.bodies[1].position);

        if (!WTime.Tick()) return;
        int steps = WTime.Advance();
        for (int i = 0; i < steps; i++) AstrophysicalSys.Step(fixedDt*timeScale,G,k_contact,k_absorb);
        
        
        for (int i = 0; i < AstrophysicalSys.bodies.Count; i++)
        {
            Body bodyState = AstrophysicalSys.bodies[i];
            if (bodyState.mass <= 0f)
            {
                bodyCfgs[i].objTsf.gameObject.SetActive(false);
                continue;
            }
            Vector3 pos = new((float)bodyState.position.x, (float)bodyState.position.y, 0);
            bodyCfgs[i].objTsf.position = pos;
            //bodyCfgs[i].positionX = bodyState.position.x;
            //bodyCfgs[i].positionY = bodyState.position.y;

            bodyCfgs[i].velocityX = bodyState.velocity.x;
            bodyCfgs[i].velocityY = bodyState.velocity.y;
            bodyCfgs[i].mass = bodyState.mass;
            bodyCfgs[i].radius = bodyState.radius;
            bodyCfgs[i].objTsf.localScale = new Vector3((float)bodyState.radius, (float)bodyState.radius, (float)bodyState.radius);
        }
    }

    public void BuildBodys()
    {
        AstrophysicalSys.bodies.Clear();
        foreach (BodyCfg bodyCfg in bodyCfgs)
        {
            Body body = new(
                id: bodyCfg.id,
                mass: bodyCfg.mass,
                position: new Vec2Double(bodyCfg.objTsf.position.x, bodyCfg.objTsf.position.y),
                velocity: new Vec2Double(bodyCfg.velocityX, bodyCfg.velocityY),
                angularVelocity: 0f,//bodyCfg.angularVelocity,
                radius: AstrophysicalSys.radiusFromMass(bodyCfg.mass)//bodyCfg.radius
            );
            AstrophysicalSys.AddBodys(body);
        }
    }
}
