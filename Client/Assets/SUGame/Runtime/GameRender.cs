
using System.Collections.Generic;
using UnityEngine;

public class GameRender
{
    public void Interpolate(List<GameMode> gameMode, float interpolating)
    {
        if (gameMode.Count == 0 || gameMode[0].worlds.Count == 0 ||gameMode[0].worlds[0].maps.Count==0) return;
        DrawGizmos.Instance.gameMode = gameMode[0];
        
    }
    
    
}
