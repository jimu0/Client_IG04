using System;
using IGC.Engine;

namespace SUGame.Simulation
{
    /// <summary>
    /// Axis-Aligned Square（轴对齐正方形）
    /// - 用于碰撞范围、区域判定
    /// - 中心对齐
    /// </summary>
    public struct Square : IEquatable<Square>
    {
        public float halfSize;

        private const float TOLERANCE = 1e-6f;

        // ─────────────────────────────────────
        // 构造
        // ─────────────────────────────────────
        public Square(float size)
        {
            halfSize = size * 0.5f;
        }

        // ─────────────────────────────────────
        // 边界访问器（相对于中心）
        // ─────────────────────────────────────
        public float Left   => -halfSize;
        public float Right  => halfSize;
        public float Top    => halfSize;
        public float Bottom => -halfSize;

        // ─────────────────────────────────────
        // 碰撞 / 包含检测
        // ─────────────────────────────────────

        /// <summary>
        /// 判断点是否在正方形内（以中心为原点）
        /// </summary>
        public bool Contains(Vec2 point)
        {
            return point.x >= Left && point.x <= Right
                && point.y >= Bottom && point.y <= Top;
        }

        /// <summary>
        /// 判断两个正方形是否相交（中心对齐）
        /// </summary>
        public bool Intersects(Square other)
        {
            return !(other.Left > Right ||
                     other.Right < Left ||
                     other.Top < Bottom ||
                     other.Bottom > Top);
        }

        // ─────────────────────────────────────
        // 相等判断
        // ─────────────────────────────────────
        public bool Equals(Square other)
        {
            return MathF.Abs(halfSize - other.halfSize) < TOLERANCE;
        }

        public override bool Equals(object obj)
        {
            return obj is Square other && Equals(other);
        }

        public override int GetHashCode()
        {
            return halfSize.GetHashCode();
        }

        public static bool operator ==(Square a, Square b) => a.Equals(b);
        public static bool operator !=(Square a, Square b) => !a.Equals(b);

        // ─────────────────────────────────────
        // Debug
        // ─────────────────────────────────────
        public override string ToString()
        {
            return $"Square(halfSize: {halfSize})";
        }
    }
}
