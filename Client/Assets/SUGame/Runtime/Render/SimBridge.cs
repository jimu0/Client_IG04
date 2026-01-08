using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreSim_IG04.SUGame;
using GameMode = CoreSim_IG04.SUGame.GameMode;
using CoreSim_IG04;
using CoreSim_IG04.SUEngine;
using CoreSim_IG04.SUGame;
using Time = CoreSim_IG04.SUEngine.Time;

public class SimBridge : MonoBehaviour
{
    
    double accumulator;
    static CoreSim_IG04.SUGame.GameModeSettings gameModeSettings = new ();
    static CoreSim_IG04.SUGame.GameMode gameMode = new CoreSim_IG04.SUGame.GameMode(gameModeSettings);
    public CoreSim_IG04.SUGame.Simulate sim = new();
    
    //List<InputSample> inputBuffer = new();
    private void Awake()
    {
        
        foreach (MonoBehaviour mb in FindObjectsOfType<MonoBehaviour>(true))
        {
            if (mb is ISim listener) sim.AddListener(listener);
        }
    }

    void Start()
    {
        Debug.Log(SimTest.Hello());
        //Debug.Log($"Time.accum:{Time.accum}");
        //Debug.Log($"Time.fixedDt:{Time.fixedDt}");
    }

    void Update()
    {
        //Debug.Log($"Time.accum:{Time.Tick(UnityEngine.Time.deltaTime)}");
        if (CoreSim_IG04.SUEngine.Time.Tick())
        {
            //Debug.Log("AAA");
            sim.Tick();
        }
    }
}
