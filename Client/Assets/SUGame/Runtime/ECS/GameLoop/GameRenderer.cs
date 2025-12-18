
using System.Collections.Generic;
using UnityEngine;

public class GameRenderer
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Interpolate(List<GameMode> gameMode, float interpolating)
    {
        if (gameMode.Count == 0 || gameMode[0].worlds.Count == 0 ||gameMode[0].worlds[0].maps.Count==0) return;
        DrawGizmos.Instance.gameMode = gameMode[0];
        
    }
    
    
}
