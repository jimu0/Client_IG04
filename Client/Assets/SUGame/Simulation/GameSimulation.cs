using System;
using System.Collections.Generic;
// Simulation 层禁用：
// - UnityEngine
// - Time
// - Transform
// - MonoBehaviour
// - Vector2 / Vector3
// - Mathf

public class GameSimulation
{
    private GameInput gameInput;
    private List<GameMode> gameMode;
    private float fixedDt;
    private bool firstTime = true;

    private int a=0;
    public void Step(List<GameMode> mode,GameInput input,float dt)
    {
        gameMode = mode;
        gameInput = input;
        fixedDt = dt;
        if (firstTime)
        {
            Start();
            firstTime = false;
        }
        else Step(dt);
    }

    private void Start()
    {
        //if (gameMode == null) gameMode = new List<GameMode>();
        if(gameMode.Count<=0)gameMode.Add(new GameMode());
        gameMode[0].CreateWorld(1,10);
        gameMode[0].SetPlayerUnit(0, 0);
    }
    
    private void Step(float dt)
    {
        //a ++;
        //gameMode[0].worlds[0].maps[0].SetTile(0,0,a);
        //PlayerInput2Unit
        gameMode[0].PlayerInput2Unit(gameInput, dt);
    }
    
    
}
