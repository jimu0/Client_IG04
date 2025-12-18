using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Gizmos.color = Color.green;
        for (int y = 0; y < gameMode.worlds[0].maps[0].GetHeight(); y++)
        {
            for (int x = 0; x < gameMode.worlds[0].maps[0].GetWidth(); x++)
            {
                Gizmos.DrawSphere(new Vector3(x,0,y), 0.2f); // 半径 0.2 米
            }
        }
        Gizmos.DrawSphere(new Vector3(gameMode.worlds[0].maps[0].GetTile(0,0),0,0), 0.5f);
    }
}
