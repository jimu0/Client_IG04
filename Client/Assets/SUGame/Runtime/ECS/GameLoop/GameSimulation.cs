
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameSimulation
{
    private static GameInput gameInput;
    private static List<GameMode> gameMode;
    private static float fixedDt;
    private static bool firstTime = true;

    private static int a=0;
    public static void Step(List<GameMode> mode,GameInput input,float dt)
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

    private static void Start()
    {
        gameMode[0].CreateWorld(1);
    }
    
    private static void Step(float dt)
    {
        a ++;
        gameMode[0].worlds[0].maps[0].SetTile(0,0,a);
    }
}
