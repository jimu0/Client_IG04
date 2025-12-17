using UnityEngine;

public class VariateWatcher
{
    float last = float.NaN;
    readonly float eps;

    public VariateWatcher(float epsilon = 0.001f)
    {
        eps = epsilon;
    }

    public bool FloatCheck(float value, out float changedValue)
    {
        if (!float.IsNaN(last) && Mathf.Abs(value - last) < eps)
        {
            changedValue = last;
            return false;
        }

        last = value;
        changedValue = value;
        return true;
    }
}
