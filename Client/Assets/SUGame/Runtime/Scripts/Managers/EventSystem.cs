using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏事件常量定义
/// </summary>
public static class GameEvent
{
    public static readonly string PLAYER_LOGIN = "PLAYER_LOGIN";
    public static readonly string LEVEL_COMPLETE = "LEVEL_COMPLETE";
    public static readonly string SCENE_LOADED = "SCENE_LOADED";
    public static readonly string GAME_PAUSED = "GAME_PAUSED";
    public static readonly string GAME_RESUMED = "GAME_RESUMED";
    public static readonly string AUDIO_VOLUME_CHANGED = "AUDIO_VOLUME_CHANGED";
}

/// <summary>
/// 全局事件系统 - 提供解耦的事件通信机制
/// 功能：
/// 1. 事件订阅和取消订阅
/// 2. 事件触发和传递
/// 3. 类型安全的事件处理
/// </summary>
public class EventSystem : Singleton<EventSystem>
{
    private Dictionary<string, Action<object>> _events = new Dictionary<string, Action<object>>();
    
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        Debug.Log("[EventSystem] 事件系统初始化完成");
    }
    
    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void Subscribe(string eventName, Action<object> callback)
    {
        if (callback == null)
        {
            Debug.LogWarning($"[EventSystem] 尝试订阅空回调到事件: {eventName}");
            return;
        }
        
        if (!_events.ContainsKey(eventName))
            _events.Add(eventName, callback);
        else
            _events[eventName] += callback;
            
        Debug.Log($"[EventSystem] 订阅事件: {eventName}，当前订阅者数量: {_events[eventName].GetInvocationList().Length}");
    }
    
    /// <summary>
    /// 取消订阅事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void Unsubscribe(string eventName, Action<object> callback)
    {
        if (callback == null || !_events.ContainsKey(eventName))
            return;
            
        _events[eventName] -= callback;
        
        // 如果没有订阅者了，移除事件键
        if (_events[eventName] == null)
        {
            _events.Remove(eventName);
            Debug.Log($"[EventSystem] 移除事件: {eventName}（无订阅者）");
        }
        else
        {
            Debug.Log($"[EventSystem] 取消订阅事件: {eventName}，剩余订阅者数量: {_events[eventName].GetInvocationList().Length}");
        }
    }
    
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="data">事件数据</param>
    public void Trigger(string eventName, object data = null)
    {
        if (_events.ContainsKey(eventName))
        {
            try
            {
                _events[eventName]?.Invoke(data);
                Debug.Log($"[EventSystem] 触发事件: {eventName}，数据类型: {data?.GetType()?.Name ?? "null"}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[EventSystem] 触发事件 {eventName} 时发生错误: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"[EventSystem] 尝试触发未订阅的事件: {eventName}");
        }
    }
    
    /// <summary>
    /// 清除所有事件订阅
    /// </summary>
    public void ClearAllEvents()
    {
        int eventCount = _events.Count;
        _events.Clear();
        Debug.Log($"[EventSystem] 清除了 {eventCount} 个事件的所有订阅");
    }
    
    /// <summary>
    /// 获取事件订阅者数量
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <returns>订阅者数量</returns>
    public int GetSubscriberCount(string eventName)
    {
        if (_events.ContainsKey(eventName) && _events[eventName] != null)
            return _events[eventName].GetInvocationList().Length;
        return 0;
    }
    
    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        ClearAllEvents();
        base.OnSingletonDestroy();
        Debug.Log("[EventSystem] 事件系统已清理");
    }
}