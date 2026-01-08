using System.Collections.Generic;
using System.Linq;
using SUGame.Simulation;

public static class InputResolver
{
    public static GameInput Resolve(List<RawInputSample> events, Vec2 currentMove)
    {
        GameInput gi = new()
        {
            moveValue = currentMove,
            attackTriggered = events.Count > 0
        };

        return gi;
    }
    
    
    
    private static Vec2 ResolveMove(List<RawInputSample> samples)
    {
        if (samples.Count == 0)
            return Vec2.Zero;

        //return samples[^1].move;
        
        Vec2 accumulated = Vec2.Zero;
        float totalWeight = 0f;

        for (int i = 0; i < samples.Count - 1; i++)
        {
            float dt = samples[i + 1].time - samples[i].time;
            if (dt <= 0f) continue;

            accumulated += samples[i].move * dt;
            totalWeight += dt;
        }

        // 处理最后一个 sample（假定延续到 dt 结束）
        accumulated += samples[^1].move;
        totalWeight += 1f;

        if (totalWeight <= 0f)
            return Vec2.Zero;

        return accumulated / totalWeight;
    }

    private static bool ResolveAttack(List<RawInputSample> samples)
    {
        return samples.Any(s => s.attackDown);
    }

}