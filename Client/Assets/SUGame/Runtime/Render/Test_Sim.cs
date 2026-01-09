using System;
using System.Collections;
using System.Collections.Generic;
using IGC.Engine;
using IGC.Game;
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

    public void OnSimUpdat(in IGC.Game.GameState _state)
    {
        if(n==IGC.Game.GameState.a)Debug.Log($"{name}:OnSimUpdat._state:{n}");
        else if(n==IGC.Game.GameState.b)Debug.Log($"{name}:OnSimUpdat._state:{n}");
        else if(n==IGC.Game.GameState.c)Debug.Log($"{name}:OnSimUpdat._state:{n}");
        else Debug.Log($"{name}:OnSimUpdat._state:null");
    }
}
