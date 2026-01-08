using System.Collections;
using System.Collections.Generic;
using SUGame.Simulation;
using UnityEngine;
//using Motion = SUGame.Simulation.Motion;
using Random = System.Random;
//using Rect = SUGame.Simulation.Rect;


public class GameMode
{
    private string settings;//规则设置
    public int id = 1;
    public List<World> worlds = new();
    public Unit player;
    public World world;
    
    
    
    public void Step(GameInput input, float dt)
    {
        PlayerInput2Unit(input, dt);
        // 未来：AI、规则、胜负判断
    }
    
    
    /// <summary>
    /// 创建世界,临时
    /// </summary>
    public void CreateWorld(int mapNumber,int pawnNumber)
    {
        Random rng = new();
        
        Map map = new(1, 10, 10);
        Unit pawn = new(1);
        
        if (worlds != null && worlds.Count > mapNumber)
        {
            List<Map> maps = worlds[mapNumber].maps;
            if (maps is { Count: > 0 })
            {
                worlds[mapNumber].maps[0] = map;
            }
            else
            {
                worlds[mapNumber].maps.Add(map);
            }
        }
        else
        {
            worlds = new List<World>();
            for (int i = 0; i < mapNumber; i++)
            {
                World world = new(i,new List<Map>(),new List<Unit>());
                world.maps.Add(map);
                
                for (int j = 0; j < pawnNumber; j++)
                {
                    pawn.position.x = rng.Next(0, 10);
                    pawn.position.y = rng.Next(0, 10);
                    world.pawns.Add(pawn);
                }
                
                worlds.Add(world);
            }
        }



        
        //GameObject fogController = ResourceManager.LoadResSync<GameObject>("ArtSite_FogController");
        //Instantiate(fogController);

        // 绘制白点
        //DrawWhiteDots(world.maps[0]);
    }


    public void Changeworld(int w)
    {
        if (worlds == null || w > worlds.Count)return;
        world = worlds[w];
    }

    public void SetPlayerUnit(int w,int i)
    {
        if (worlds == null || w > worlds.Count)return;
        if(worlds[w].pawns == null || i > worlds[w].pawns.Count) return;
        player = worlds[w].pawns[i];
    }

    public void PlayerInput2Unit(GameInput input,float dt)
    {
        Debug.Log($"input:{input.moveValue}");
        if (player.id <= 0) return;
        if (input.moveValue.Length() < 0.01f) return;
        player.motion.targetVelocity = input.moveValue * 3;
        if (!input.moving)player.facing = Facing.FromVector(input.moveValue);
        player.Tick(dt);
        worlds[0].pawns[0] = player;
        //if(!v && !fireTriggerState) LookAtTarget(pawnPos); // 移动改变朝向的前提是玩家不在瞄准或射击状态以及判断是否有准星指在操作
        //if (!v) LookAtTarget(pawnPos); //移动时候不再因为是否按下开火而固定方向，除非开火期间进行了精确瞄准

    }
    
}
