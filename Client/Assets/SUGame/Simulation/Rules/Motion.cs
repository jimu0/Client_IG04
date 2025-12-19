using System;

namespace SUGame.Simulation
{
    /// <summary>
    /// 单位运动状态
    /// - 记录速度、加速度、目标速度
    /// - 提供固定步长更新方法
    /// </summary>
    public struct Motion
    {
        /// <summary>
        /// 当前速度（世界坐标系）
        /// </summary>
        public Vec2 velocity;

        /// <summary>
        /// 当前加速度
        /// </summary>
        public Vec2 acceleration;

        /// <summary>
        /// 期望速度（可以用作移动输入或 AI 控制）
        /// </summary>
        public Vec2 targetVelocity;

        /// <summary>
        /// 构造
        /// </summary>
        public Motion(Vec2 velocity, Vec2 acceleration, Vec2 targetVelocity)
        {
            this.velocity = velocity;
            this.acceleration = acceleration;
            this.targetVelocity = targetVelocity;
        }

        // ─────────────────────────────────────
        // 固定步长更新
        // ─────────────────────────────────────

        /// <summary>
        /// 更新速度（简单物理，Euler积分）
        /// </summary>
        /// <param name="dt">固定步长</param>
        public void Tick(float dt)
        {
            // 将当前速度朝 targetVelocity 靠拢
            Vec2 deltaV = targetVelocity - velocity;

            // 加速度限制（简单线性逼近）
            Vec2 dv = deltaV * dt;

            // 应用加速度
            velocity += acceleration * dt + dv;
        }

        // ─────────────────────────────────────
        // 辅助方法
        // ─────────────────────────────────────

        /// <summary>
        /// 设置新的目标速度
        /// </summary>
        public void SetTarget(Vec2 target)
        {
            targetVelocity = target;
        }

        /// <summary>
        /// 停止运动
        /// </summary>
        public void Stop()
        {
            velocity = Vec2.Zero;
            acceleration = Vec2.Zero;
            targetVelocity = Vec2.Zero;
        }

        public override string ToString()
        {
            return $"Motion(vel: {velocity}, accel: {acceleration}, targetVel: {targetVelocity})";
        }
    }
}
