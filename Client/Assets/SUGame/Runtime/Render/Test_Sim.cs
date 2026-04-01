
using Mycelia;
using UnityEngine;
using Input = Mycelia.Input;

public class Test_Sim : MonoBehaviour,ISim,IRender
{
    
    private void DebugLog(WorldState state)
    {
        Debug.Log($"StateTest : {state.debugText}");
    }


    public SimPhase Phase => SimPhase.Step;
    public void OnSimStart(in Input input, ref WorldState state)
    {
        //state.debugText = $"hello world!";
    }

    public void OnSimStep(in Input input, ref WorldState state)
    {
        
    }
    
    
    public void OnRender(in WorldState state)
    {
        //DebugLog(state);
    }
}

