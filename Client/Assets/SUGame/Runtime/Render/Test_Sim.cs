
using Mycelia;
using UnityEngine;

public class Test_Sim : Singleton<Test_Sim>, ISim, IRender
{
    
    private void DebugLog(WorldState state)
    {
        Debug.Log($"StateTest : {state.debugText}");
    }


    public SimPhase Phase => SimPhase.Step;
    public void OnSimStart(in CtrlInput input, ref WorldState state)
    {
        //state.debugText = $"hello world!";
    }

    public void OnSimStep(in CtrlInput input, ref WorldState state)
    {
        
    }
    
    
    public void OnRender(in WorldState state)
    {
        //DebugLog(state);
    }
}

