using System;

namespace SUGame.Simulation
{
    /// <summary>
    /// Axis-Aligned Rectangle（轴对齐矩形）
    /// - 用于碰撞范围、区域判定等
    /// - 中心点对齐
    /// </summary>
    public struct Rect : IEquatable<Rect>
    {
        public float halfWidth;
        public float halfHeight;

        // ─────────────────────────────────────
        // 构造
        // ─────────────────────────────────────
        public Rect(float width, float height)
        {
            halfWidth = width * 0.5f;
            halfHeight = height * 0.5f;
        }
        // 常量
        public static Rect Zero => new Rect(0f, 0f);
        public static Rect One => new Rect(1f, 1f);
        
        // ─────────────────────────────────────
        // 边界访问器（相对于中心）
        // ─────────────────────────────────────
        public float Left   => -halfWidth;
        public float Right  => halfWidth;
        public float Top    => halfHeight;
        public float Bottom => -halfHeight;

        // ─────────────────────────────────────
        // 碰撞 / 包含检测
        // ─────────────────────────────────────

        /// <summary>
        /// 判断点是否在矩形内（以中心为原点）
        /// </summary>
        public bool Contains(float x, float y)
        {
            return x >= Left && x <= Right && y >= Bottom && y <= Top;
        }

        /// <summary>
        /// 判断两个矩形是否相交（中心对齐）
        /// </summary>
        public bool Intersects(Rect other)
        {
            return !(other.Left > Right ||
                     other.Right < Left ||
                     other.Top < Bottom ||
                     other.Bottom > Top);
        }

        // ─────────────────────────────────────
        // 相等判断
        // ─────────────────────────────────────
        public bool Equals(Rect other)
        {
            return MathF.Abs(halfWidth - other.halfWidth) < 1e-6f && MathF.Abs(halfHeight - other.halfHeight) < 1e-6f;
        }

        public override bool Equals(object obj)
        {
            return obj is Rect other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(halfWidth, halfHeight);
        }

        public static bool operator ==(Rect a, Rect b) => a.Equals(b);
        public static bool operator !=(Rect a, Rect b) => !a.Equals(b);

        // ─────────────────────────────────────
        // Debug
        // ─────────────────────────────────────
        public override string ToString()
        {
            return $"Rect(halfWidth: {halfWidth}, halfHeight: {halfHeight})";
        }
    }
}
