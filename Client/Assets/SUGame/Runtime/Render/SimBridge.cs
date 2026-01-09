using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IGC.Engine;
using IGC.Game;

public class SimBridge : MonoBehaviour
{
    
    double accumulator;
    static GameModeSettings gameModeSettings = new ();
    static IGC.Game.GameMode gameMode = new IGC.Game.GameMode(gameModeSettings);

    //List<InputSample> inputBuffer = new();
    private void Awake()
    {
        foreach (MonoBehaviour mb in FindObjectsOfType<MonoBehaviour>(true))
        {
            if (mb is ISim listener) Igc.AddListener(listener);
        }
    }

    void Start()
    {
        //Debug.Log($"Time.accum:{Time.accum}");
        //Debug.Log($"Time.fixedDt:{Time.fixedDt}");
    }

    void Update()
    {
        Igc.Simulate();
    }
}
