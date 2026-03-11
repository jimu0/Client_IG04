using System;
using Mycelia;

namespace SUGame.Simulation
{
    /// <summary>
    /// 单位朝向（2D）
    /// - 可以表示为角度或单位向量
    /// - 用于移动、旋转、AI 朝向判断
    /// </summary>
    public struct Facing : IEquatable<Facing>
    {
        /// <summary>
        /// 弧度表示的角度，0 表示向右，顺时针为正
        /// </summary>
        public float angleRad;

        private const float TOLERANCE = 1e-6f;

        // ─────────────────────────────────────
        // 构造
        // ─────────────────────────────────────

        public Facing(float angleRad)
        {
            this.angleRad = angleRad;
        }

        /// <summary>
        /// 根据单位向量设置朝向
        /// </summary>
        public static Facing FromVector(Vec2 dir)
        {
            Vec2 norm = dir.Normalized();
            return new Facing(MathF.Atan2(norm.y, norm.x));
        }

        // ─────────────────────────────────────
        // 转换为向量
        // ─────────────────────────────────────

        /// <summary>
        /// 获取单位向量表示
        /// </summary>
        public Vec2 ToVector()
        {
            return new Vec2(MathF.Cos(angleRad), MathF.Sin(angleRad));
        }

        // ─────────────────────────────────────
        // 旋转
        // ─────────────────────────────────────

        /// <summary>
        /// 顺时针旋转指定弧度
        /// </summary>
        public Facing Rotate(float deltaRad)
        {
            return new Facing(angleRad + deltaRad);
        }

        // ─────────────────────────────────────
        // 相等判断
        // ─────────────────────────────────────

        public bool Equals(Facing other)
        {
            return MathF.Abs(angleRad - other.angleRad) < TOLERANCE;
        }

        public override bool Equals(object obj)
        {
            return obj is Facing other && Equals(other);
        }

        public override int GetHashCode()
        {
            return angleRad.GetHashCode();
        }

        public static bool operator ==(Facing a, Facing b) => a.Equals(b);
        public static bool operator !=(Facing a, Facing b) => !a.Equals(b);

        // ─────────────────────────────────────
        // Debug
        // ─────────────────────────────────────

        public override string ToString()
        {
            return $"Facing({angleRad:0.###} rad)";
        }
    }
}
