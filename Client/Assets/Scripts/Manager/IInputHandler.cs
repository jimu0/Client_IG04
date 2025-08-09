using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandler
{
    // 返回该脚本需要的所有按键绑定
    IEnumerable<KeyBind> GetKeyBinds();
}

// 按键绑定配置
public struct KeyBind
{
    public KeyCode key;          // 主按键
    public bool requireCtrl;     // 需要Ctrl
    public bool requireShift;    // 需要Shift
    public bool requireAlt;      // 需要Alt
    public Action onTriggered;   // 触发时的回调
    public string description;   // 描述（用于调试）
    public int priority;         // 数值越大优先级越高
}