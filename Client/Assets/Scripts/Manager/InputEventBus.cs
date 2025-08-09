using System;
using System.Collections.Generic;

// 输入事件系统（独立于InputManager）
public static class InputEventBus
{
    // 注册/注销事件
    public static event Action<IEnumerable<KeyBind>> OnBindsRegistered;
    public static event Action<Func<KeyBind, bool>> OnBindsUnregistered;

    // 触发事件
    public static void RegisterBinds(IEnumerable<KeyBind> binds) 
        => OnBindsRegistered?.Invoke(binds);
    
    public static void UnregisterBinds(Func<KeyBind, bool> matchCondition) 
        => OnBindsUnregistered?.Invoke(matchCondition);
}
