using System;
using System.Collections.Generic;
using SUGame.Simulation;
using Mycelia;
public struct World
{
    public int id;
    public List<Map> maps;
    public List<Unit> pawns;
    
    public World(int id, List<Map> maps, List<Unit> pawns)
    {
        this.id = id;
        this.maps = maps;
        this.pawns = pawns;
    }

    public void CreatePawn(Vec2 vec2)
    {
        // 创建一个单位
        Unit pawn = new(
            id: -1,
            position: vec2,
            facing: new Facing(0),
            hitbox: Rect.One,
            motion: new Motion(Vec2.Zero, Vec2.Zero, Vec2.Zero)
        );
        pawns.Add(pawn);
    }


    public override string ToString()
    {
        return $"World:{id},maps:{maps.Count},pawns{pawns.Count}";
    }
}
