using System.Collections.Generic;
using IGC.Engine;

public class InputBuffer
{
    private readonly Queue<RawInputSample> samples = new();

    public Vec2 CurrentMove { get; private set; }

    public void Push(RawInputSample sample)
    {
        CurrentMove = sample.move; // 状态：直接覆盖
        if (sample.attackDown) samples.Enqueue(sample); // 事件：才入队
    }

    public List<RawInputSample> Consume(float fromTime, float toTime)
    {
        List<RawInputSample> result = new();
        while (samples.Count > 0)
        {
            var s = samples.Peek();
            if (s.time < fromTime) { samples.Dequeue(); continue; }
            if (s.time >= toTime) break;
            result.Add(samples.Dequeue());
        }
        return result;
    }
}
