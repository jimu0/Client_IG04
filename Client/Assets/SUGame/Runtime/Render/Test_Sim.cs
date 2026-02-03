
using System.Security.AccessControl;
using UnityEngine;
using IGCEngine;

public class Test_Sim : MonoBehaviour
{
    
    public GameObject player;
    public GameObject[] totems;
    private void Start()
    {
        
    }

    private void Update()
    {
        //DebugLog();
        
    }
    
    private void DebugLog()
    {
        //Debug.Log($"血液：{Igc.GetWorldState.tick}");
        //Debug.Log($"TestTxt:{Igc.GetWorldState.debugText}"); 

        Debug.Log($"TestPlayer{Igc.GetWorldState.playerState.position}");

    }

    private void GetState()
    {
        //totems=
    }

}

