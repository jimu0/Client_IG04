using System.Collections;
using System.Collections.Generic;
using Mycelia;
using UnityEngine;

public class DebugDraw : Singleton<DebugDraw>, ISim, IRender
{
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
    }

    public void OnSimStep(in CtrlInput input, ref WorldState state)
    {
        // string a = $"WangWangWang!!!+{state.extensions.Get<Test_Draw>().i++}";
        // state.extensions.Get<Test_Draw>().txt = a;
    }

    
    public void OnRender(in WorldState state)
    {
        //Debug.Log(state.extensions.Get<Test_Draw>().txt);
    }
    
}
