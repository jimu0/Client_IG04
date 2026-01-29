using UnityEngine;
using IGCEngine;

public class SimBridge : MonoBehaviour
{
    private void Awake()
    {
        Igc.Simulate_Awake();
    }
    void Start()
    {
        Igc.Simulate_Start();
    }
    void Update()
    {
        PlayerInput();
        Igc.Simulate_Update();
    }


    void PlayerInput()
    {
        // if ()
        // {
        //     
        // }
    }
}
