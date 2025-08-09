using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputManager() { }
    public static InputManager Instance { get; private set; }
    
    private readonly List<KeyBind> keyBinds = new();// 存储所有按键绑定

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    // // 注册按键绑定
    // public void RegisterBinds(IEnumerable<KeyBind> binds)
    // {
    //     // 检查重复绑定
    //     IEnumerable<KeyBind> newBinds = binds as KeyBind[] ?? binds.ToArray();
    //     foreach (KeyBind newBind in newBinds)
    //     {
    //         if (keyBinds.Any(existing => 
    //                 existing.key == newBind.key &&
    //                 existing.requireCtrl == newBind.requireCtrl &&
    //                 existing.requireShift == newBind.requireShift))
    //         {
    //             Debug.LogWarning($"按键冲突: {newBind.description}");
    //         }
    //     }
    //     keyBinds.AddRange(newBinds);
    //     
    // }
    //
    // // 注销特定来源的所有绑定
    // public void UnregisterBinds(Func<KeyBind, bool> matchCondition)
    // {
    //     keyBinds.RemoveAll(b => matchCondition(b));
    // }
    
    private void HandleRegisterBinds(IEnumerable<KeyBind> binds)
    {
        IEnumerable<KeyBind> newBinds = binds as KeyBind[] ?? binds.ToArray();
        foreach (KeyBind newBind in newBinds)
        {
            if (keyBinds.Any(existing => 
                    existing.key == newBind.key &&
                    existing.requireCtrl == newBind.requireCtrl &&
                    existing.requireShift == newBind.requireShift))
            {
                Debug.LogWarning($"按键冲突: {newBind.description}");
            }
        }
        keyBinds.AddRange(newBinds);
    }
    private void HandleUnregisterBinds(Func<KeyBind, bool> matchCondition)
    {
        keyBinds.RemoveAll(b => matchCondition(b));
    }
    void OnEnable()
    {
        InputEventBus.OnBindsRegistered += HandleRegisterBinds;
        InputEventBus.OnBindsUnregistered += HandleUnregisterBinds;
    }

    void OnDisable()
    {
        InputEventBus.OnBindsRegistered -= HandleRegisterBinds;
        InputEventBus.OnBindsUnregistered -= HandleUnregisterBinds;
    }
    
    void Update()
    {
        // 排序处理
        foreach (KeyBind bind in keyBinds.OrderByDescending(b => b.priority))
        {
            if (CheckKeyPressed(bind))
            {
                bind.onTriggered?.Invoke();
                break;
            }
        }
        // foreach (var bind in keyBinds)
        // {
        //     if (CheckKeyPressed(bind))
        //     {
        //         bind.onTriggered?.Invoke();
        //     }
        // }
    }

    // 检测按键组合是否按下
    private bool CheckKeyPressed(KeyBind bind)
    {
        bool keyDown = Input.GetKeyDown(bind.key);
        bool ctrlOk = !bind.requireCtrl || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool shiftOk = !bind.requireShift || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool altOk = !bind.requireAlt || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        return keyDown && ctrlOk && shiftOk && altOk;
    }
    
    // 动态修改按键绑定
    public void RebindKey(KeyBind originalBind, KeyCode newKey)
    {
        int index = keyBinds.FindIndex(b => b.Equals(originalBind));
        if (index >= 0)
        {
            KeyBind newBind = originalBind;
            newBind.key = newKey;
            keyBinds[index] = newBind;
        }
    }
    


    
    // 调试用：打印所有绑定
    public void LogAllBinds()
    {
        foreach (KeyBind bind in keyBinds)
        {
            Debug.Log($"{bind.description} - Key: {bind.key}, Ctrl: {bind.requireCtrl}");
        }
    }
}