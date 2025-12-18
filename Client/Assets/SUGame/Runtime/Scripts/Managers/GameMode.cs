using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Diagnostics;

public class GameMode
{
    private string settings;//规则设置
    public int id = 1;
    public List<World> worlds = new();
    public List<Pawn> pawns = new();
    public Pawn playerPawn;
    void Start()
    {
        
        //GameObject bulletPool = ResourceManager.LoadResSync<GameObject>("ArtProp_BullePool");
        //Instantiate(bulletPool);

    }
    

    // private Pawn ResPlayer()
    // {
    //
    //     Pawn player = ResourceManager.LoadResSync<Pawn>("Player_Pawn_BT_Player1");
    //     player.name = "playerPawnObj";
    //
    //     return Instantiate(player);
    //     
    // }
    
    /// <summary>
    /// 创建世界,临时
    /// </summary>
    public void CreateWorld(int number)
    {
        
        Map map = new(1, 10, 10);
        if (worlds != null && worlds.Count > number)
        {
            List<Map> maps = worlds[number].maps;
            if (maps is { Count: > 0 })
            {
                worlds[number].maps[0] = map;
            }
            else
            {
                worlds[number].maps.Add(map);
            }
        }
        else
        {
            worlds = new();
            for (int i = 0; i < number; i++)
            {
                World world = new(i);
                world.maps.Add(map);
                worlds.Add(world);
            }
        }



        
        //GameObject fogController = ResourceManager.LoadResSync<GameObject>("ArtSite_FogController");
        //Instantiate(fogController);

        // 绘制白点
        //DrawWhiteDots(world.maps[0]);
    }


    private void Update()
    {
        //PlayerUpdate
        //EntityUpdate
    }


    private void DrawWhiteDots(Map map)
    {
        for (int x = 0; x < map.GetWidth(); x++)
        {
            for (int y = 0; y < map.GetHeight(); y++)
            {
                // 计算世界坐标（假设每个瓦片大小为1单位）
                Vector3 position = new Vector3(x, 0, y);
            
                // 使用 Debug.DrawLine 绘制白点（短线段模拟点）
                Debug.DrawLine(position, position + Vector3.up * 0.2f, Color.white, 100f);
            
                // 或者使用 Gizmos（需在 OnDrawGizmos 中调用）
                // Gizmos.color = Color.white;
                // Gizmos.DrawSphere(position, 0.1f);
            }
        }
    }
}
