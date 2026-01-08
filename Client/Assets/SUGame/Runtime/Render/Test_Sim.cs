using System;
using System.Collections;
using System.Collections.Generic;
using CoreSim_IG04.SUGame;
using UnityEngine;

public class Test_Sim : MonoBehaviour,ISim
{
    private string name;
    public int n;
    private void Start()
    {
        name = gameObject.name;
        //Simulate.RegisterListener
    }

    public void OnSimUpdat(in CoreSim_IG04.SUGame.GameState _state)
    {
        if(n==CoreSim_IG04.SUGame.GameState.a)Debug.Log($"{name}:OnSimUpdat._state:{n}");
        else if(n==CoreSim_IG04.SUGame.GameState.b)Debug.Log($"{name}:OnSimUpdat._state:{n}");
        else if(n==CoreSim_IG04.SUGame.GameState.c)Debug.Log($"{name}:OnSimUpdat._state:{n}");
        else Debug.Log($"{name}:OnSimUpdat._state:null");
    }
}
