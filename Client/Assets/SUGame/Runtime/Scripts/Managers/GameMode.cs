using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Diagnostics;

public class GameMode : Singleton<GameMode>
{
    public GameObject playerObj;
    //public GameObject world;
    //public GameObject[];

    void Start()
    {
        playerObj = ResPlayer();
        GenerateLevel();

        GameObject bulletPool = ResourceManager.LoadResSync<GameObject>("ArtProp_BullePool");
        Instantiate(bulletPool);

    }
    
    private GameObject ResPlayer()
    {

        GameObject player = ResourceManager.LoadResSync<GameObject>("Player_Pawn_BT_Player1");


        return Instantiate(player);
        
    }
    
    /// <summary>
    /// 生成世界
    /// </summary>
    public void GenerateLevel()
    {
        GameObject level0 = new() { name = "Level0" };
        World world = level0.AddComponent<World>();
        world.maps.Add(new Map(1, 10, 10));
        
        GameObject fogController = ResourceManager.LoadResSync<GameObject>("ArtSite_FogController");
        Instantiate(fogController);
        // //DontDestroyOnLoad(fogController);
        // GameObject fogZVolume = fogController.GetComponent<FogController>().fogZVolume;
        // List<ConstraintSource> css = new List<ConstraintSource>();
        // ConstraintSource cs=new()
        // {
        //     sourceTransform = Camera.main.transform,
        //     weight = 1
        // };
        // css.Add(cs);
        // fogZVolume.GetComponent<ParentConstraint>().SetSources(css);
        
        
        // 绘制白点
        DrawWhiteDots(world.maps[0]);
        //throw new System.NotImplementedException();
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
