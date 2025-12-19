using System;
using SUGame.Simulation.Math;
using SUGame.Simulation.Spatial;

namespace SUGame.Simulation.Entity
{
    public struct Unit
    {
        public Vec2 position;
        public Facing facing;
        public Rect hitbox;
        public Motion motion;

        public Unit(Vec2 position, Facing facing, Rect hitbox, Motion motion)
        {
            this.position = position;
            this.facing = facing;
            this.hitbox = hitbox;
            this.motion = motion;
        }

        public void Tick(float dt)
        {
            // 更新速度
            motion.Tick(dt);

            // 更新位置
            position += motion.velocity * dt;

            // 根据速度更新朝向（可选）
            if (motion.velocity.LengthSq() > 1e-6f)
                facing = Facing.FromVector(motion.velocity);
        }

        public bool Intersects(Unit other)
        {
            Rect a = hitbox;
            Rect b = other.hitbox;

            Vec2 deltaA = position;
            Vec2 deltaB = other.position;

            return !(deltaB.x + b.Left > deltaA.x + a.Right ||
                     deltaB.x + b.Right < deltaA.x + a.Left ||
                     deltaB.y + b.Top < deltaA.y + a.Bottom ||
                     deltaB.y + b.Bottom > deltaA.y + a.Top);
        }

        public override string ToString()
        {
            return $"Unit(pos: {position}, facing: {facing}, motion: {motion}, hitbox: {hitbox})";
        }
    }
}