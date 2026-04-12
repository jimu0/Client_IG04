using System.Collections;
using System.Collections.Generic;
using Mycelia;
using UnityEngine;

public class DebugDraw : Singleton<DebugDraw>, ISim, IRender
{
    public class DrawRole
    {
        public Vector3 center;
        public Vector2 size;
        public Color color;
        public DrawRole()
        {
            center = Vector3.zero;
            size = Vector2.one;
            color = Color.white;
        }
        public DrawRole(Vector3 v, Vector2 s, Color c)
        {
            center= v;
            size = s;
            color = c;
        }
    }
    public readonly DrawRole[] drawRole = new DrawRole[10];
    

    public class Test_Draw
    {
        public string txt = "hi?";
        public int i;
    }

    public SimPhase Phase => SimPhase.Step;
    public void OnSimStart(in CtrlInput input, ref WorldState state)
    {
        // state.extensions = new StateExtensions();
        // state.extensions.Register(new Test_Draw());
        for (int i = 0; i < drawRole.Length; i++)
        {
            drawRole[i] = new DrawRole
            {
                center = new Vector3(state.roleStates[i].tsf.postion.x, 0, state.roleStates[i].tsf.postion.y),
                size = new Vector2(state.roleStates[i].tsf.scale.x, state.roleStates[i].tsf.scale.y)
            };
            if (i == 0) drawRole[i].color = Color.green;
        }
    }

    public void OnSimStep(in CtrlInput input, ref WorldState state)
    {
        // string a = $"WangWangWang!!!+{state.extensions.Get<Test_Draw>().i++}";
        // state.extensions.Get<Test_Draw>().txt = a;
        drawRole[0].center = new Vector3(state.roleStates[0].tsf.postion.x,0,state.roleStates[0].tsf.postion.y);
        for (int i = 0; i < drawRole.Length; i++)
        {
            //TODO:碰撞检测改画线颜色
        }
    }

    
    public void OnRender(in WorldState state)
    {
        //Debug.Log(state.extensions.Get<Test_Draw>().txt);
        for (int i = 0; i < drawRole.Length; i++)
        {
            var role = drawRole[i];
            DrawRectXZ(role.center, role.size, role.color);
        }
        
    }

    
    
    
    
    public static void DrawRectXZ(Vector3 center, Vector2 size, Color color, float duration = 0f, bool depthTest = true)
    {
        var halfX = size.x * 0.5f;
        var halfZ = size.y * 0.5f;

        var p0 = new Vector3(center.x - halfX, center.y, center.z - halfZ);
        var p1 = new Vector3(center.x + halfX, center.y, center.z - halfZ);
        var p2 = new Vector3(center.x + halfX, center.y, center.z + halfZ);
        var p3 = new Vector3(center.x - halfX, center.y, center.z + halfZ);

        Debug.DrawLine(p0, p1, color, duration, depthTest);
        Debug.DrawLine(p1, p2, color, duration, depthTest);
        Debug.DrawLine(p2, p3, color, duration, depthTest);
        Debug.DrawLine(p3, p0, color, duration, depthTest);
    }
}
