using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rect = SUGame.Simulation.Rect;

public class DrawGizmos : Singleton<DrawGizmos>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameMode gameMode;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        for (int y = 0; y < gameMode.worlds[0].maps[0].GetHeight(); y++)
        {
            for (int x = 0; x < gameMode.worlds[0].maps[0].GetWidth(); x++)
            {
                Gizmos.DrawCube(new Vector3(x,0,y),new Vector3(0.99f,0.001f,0.99f));

                //Gizmos.DrawSphere(new Vector3(x,0,y), 0.2f); // 半径 0.2 米
            }
        }
        //Gizmos.DrawSphere(new Vector3(gameMode.worlds[0].maps[0].GetTile(0,0),0,0), 0.5f);
        Gizmos.color = Color.green;
        for (int i = 0; i < gameMode.worlds[0].pawns.Count; i++)
        {
            if (i == 0) Gizmos.color = Color.green;
            else Gizmos.color = Color.yellow;
            Vector3 pawnPos = Vector3.zero;
            pawnPos.x = gameMode.worlds[0].pawns[i].position.x;
            pawnPos.z = gameMode.worlds[0].pawns[i].position.y;
            Rect pawnRect = gameMode.worlds[0].pawns[i].hitbox;
            Gizmos.DrawCube(pawnPos,new Vector3(pawnRect.halfWidth,1,pawnRect.halfHeight));
        }
        
    }
}
