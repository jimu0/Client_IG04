using System;

namespace SUGame.Simulation
{
    /// <summary>
    /// 2D 向量（Simulation Core）
    /// - 不依赖 Unity
    /// - 表示世界坐标、速度、方向等状态
    /// - 只包含当前 Simulation 需要的最小功能集
    /// </summary>
    public struct Vec2 : IEquatable<Vec2>
    {
        public float x;
        public float y;
        
        // 构造
        public Vec2(float x, float y) { this.x = x; this.y = y; }
        
        // 常量
        public static Vec2 Zero => new Vec2(0f, 0f);
        public static Vec2 One  => new Vec2(1f, 1f);
        public static Vec2 Right => new Vec2(1f, 0f);
        public static Vec2 Left  => new Vec2(-1f, 0f);
        public static Vec2 Up    => new Vec2(0f, 1f);
        public static Vec2 Down  => new Vec2(0f, -1f);

        // ─────────────────────────────────────
        // 基础运算
        // ─────────────────────────────────────

        /// <summary>
        ///  平方和
        /// </summary>
        /// <returns>返回向量长度的平方</returns>
        public float LengthSq() { return x * x + y * y; }
        /// <summary>
        /// 范数（模）
        /// </summary>
        /// <returns>返回向量的实际长度（模）</returns>
        public float Length() { return MathF.Sqrt(LengthSq()); }

        /// <summary>
        /// 归一化
        /// </summary>
        /// <returns>返回单位向量</returns>
        public Vec2 Normalized()
        {
            // float len = Length();
            // return len < 1e-6f ? Zero : new Vec2(x / len, y / len);
            
            float lenSq = LengthSq();
            if (lenSq < 1e-12f) return Zero;
            float invLen = 1.0f / MathF.Sqrt(lenSq);
            return new Vec2(x * invLen, y * invLen);
        }

        // ─────────────────────────────────────
        // 静态函数
        // ─────────────────────────────────────
        
        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>返回float</returns>
        public static float Dot(Vec2 a, Vec2 b) { return a.x * b.x + a.y * b.y; }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns>返回Vec2</returns>
        public static Vec2 Lerp(Vec2 a, Vec2 b, float t)
        {
            return new Vec2(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t
            );
        }

        // ─────────────────────────────────────
        // 运算符重载
        // ─────────────────────────────────────

        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.x + b.x, a.y + b.y);

        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.x - b.x, a.y - b.y);

        public static Vec2 operator -(Vec2 v) => new Vec2(-v.x, -v.y);

        public static Vec2 operator *(Vec2 v, float s) => new Vec2(v.x * s, v.y * s);

        public static Vec2 operator *(float s, Vec2 v) => new Vec2(v.x * s, v.y * s);

        public static Vec2 operator /(Vec2 v, float s) => new Vec2(v.x / s, v.y / s);

        // ─────────────────────────────────────
        // 相等判断（值语义）
        // ─────────────────────────────────────

        /// <summary>
        /// 分量相等比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns>返回bool</returns>
        public bool Equals(Vec2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }
        /// <summary>
        /// 多态相等性比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回bool</returns>
        public override bool Equals(object obj)
        {
            return obj is Vec2 other && Equals(other);
        }
        /// <summary>
        /// 哈希码生成
        /// </summary>
        /// <returns>返回int</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public static bool operator ==(Vec2 a, Vec2 b)
            => a.Equals(b);

        public static bool operator !=(Vec2 a, Vec2 b)
            => !a.Equals(b);

        // ─────────────────────────────────────
        // Debug
        // ─────────────────────────────────────

        public override string ToString()
        {
            return $"({x:0.###}, {y:0.###})";
        }
    }
}
