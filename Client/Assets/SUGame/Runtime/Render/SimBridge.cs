using UnityEngine;
using IGCEngine;
using SUEngine;

public class SimBridge : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Igc.InitTables(new LubanConfigService());
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
