
using System.Collections.Generic;
using SUGame.Simulation;
using UnityEngine;

public class GameRender
{
    private List<GameMode> gameMode;
    public void Interpolate(GameState state, float interpolating)
    {
        gameMode = state.gameModes;
        if (gameMode.Count == 0 || gameMode[0].worlds.Count == 0 ||gameMode[0].worlds[0].maps.Count==0) return;
        DrawGizmos.Instance.gameMode = gameMode[0];
        //Debug.Log($"{gameMode[0].player.position}");
    }
    
}
