using IGC.Engine;

public struct RawInputSample
{
    public float time;          // Time.time or unscaledTime
    public Vec2 move;           // 连续轴
    public bool attackDown;     // 这一帧是否按下
    public bool attackHeld;     // 当前是否按住（可选）
}
