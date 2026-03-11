using System;
using Mycelia;
using System.Collections.Generic;



public static class AstrophysicalSys
{
    public static List<Body> bodies = new();
    
    //public static SimTime simTime = new();
    
    private static double eps = 0.0001f;

    public static void AddBodys(Body body)
    {
        bodies.Add(body);
    }


    public static void Step(double dt,double G, double kc,double ka)
    {
        int count = bodies.Count;
        if (count < 1) return;
        //if (eps <= 0.0) return;
        
        //缓存
        Vec2Double[] accelerations = new Vec2Double[count];
        double[] mass = new double[count];
        double[] radius = new double[count];
        for (int i = 0; i < count; i++)
        {
            mass[i] = bodies[i].mass;
            radius[i] = bodies[i].radius;
        }
        
        //基于当前位置计算所有body的加速度
        for (int a = 0; a < count; a++)
        {
            if (bodies[a].mass <= 0f) continue;
            for (int b = a + 1; b < count; b++)
            {
                if (bodies[b].mass <= 0f) continue;
                Vec2Double delta = bodies[b].position - bodies[a].position;//位移向量
                double dist = delta.Length();//求模，也就是两点间距
                if (dist < 1e-6) dist = 1e-6;
                double combinedRadius = bodies[b].radius + bodies[a].radius;//半径之和
                combinedRadius = Math.Max(combinedRadius, 1e-6);
                double penetration  = 1.0 - dist / combinedRadius;//穿透程度(0~1)
                double k_eps = combinedRadius * eps;//接触软化因子,规避牛顿引力的小毛病
                Vec2Double dir = delta.Normalized();//归一化,方向
                
                // 引力（始终存在）
                //applyGravity(a, b);
                double forceG = G * bodies[a].mass * bodies[b].mass / (dist * dist + k_eps * k_eps);
                Vec2Double F_gravity = dir * forceG;
                accelerations[a] += F_gravity / bodies[a].mass;
                accelerations[b] -= F_gravity / bodies[b].mass;
                
                // 接触响应（只在 penetration > 0）
                if (penetration > 0)
                {
                    //applyContactForce(a, b);
                    double mEff = Math.Min(bodies[a].mass, bodies[b].mass);//较小的质量
                    double k_contact = kc * mEff / (dt * dt);//刚度因子
                    double contactForce = k_contact * penetration;//反作用力/接触力,排斥力,弹力
                    Vec2Double F_contact = dir * contactForce;
                    accelerations[a] -= F_contact / bodies[a].mass;
                    accelerations[b] += F_contact / bodies[b].mass;
                    
                    //applyMassTransfer(a, b);
                    double relSpeed = Vec2Double.Dot(bodies[b].velocity - bodies[a].velocity, dir);
                    double dm = ka * penetration * Math.Max(0, -relSpeed) * dt;//ka → 调节整体速率的比例系数
                    int smallId, largeId;
                    if (bodies[a].mass < bodies[b].mass)
                    {
                        smallId = a;
                        largeId = b;
                    }
                    else
                    {
                        smallId = b;
                        largeId = a;
                    }
                    dm = Math.Min(dm, bodies[smallId].mass);
                    mass[smallId] -= dm;
                    mass[largeId] += dm;
                    radius[smallId] = radiusFromMass(mass[smallId]);
                    radius[largeId] = radiusFromMass(mass[largeId]);
                    //radius[smallId] = Math.Max(radiusFromMass(mass[smallId]), 1e-6);
                    //radius[largeId] = Math.Max(radiusFromMass(mass[largeId]), 1e-6);
                    
                }
                
            }
        }

        //半隐式欧拉：先更新速度，再更新位置
        for (int i = 0; i < count; i++)
        {
            //if (bodies[i] == null) continue;
            Body body = bodies[i];
            body.velocity += accelerations[i] * dt;
            body.position += body.velocity * dt;
            body.radius = radius[i];
            body.mass = mass[i];
            // if (mass[i] > 0)
            // {
            //     bodies[i] = body;
            // }
            // else
            // {
            //     //bodies.RemoveAt(i);
            // }
            bodies[i] = body;
        }
        
    }

    public static double radiusFromMass(double mass)
    {
        return Math.Pow(mass / 1.00, 1.0/3.0);
    }
}
