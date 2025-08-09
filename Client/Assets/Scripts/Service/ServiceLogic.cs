//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLogic : MonoBehaviour,IInputHandler
{

    [SerializeField] private float moveSpeed = 5f;
    public IEnumerable<KeyBind> GetKeyBinds()
    {
        return new[]
        {
            new KeyBind
            {
                key = KeyCode.W,
                requireCtrl = true,
                requireShift = false,
                requireAlt = false,
                onTriggered = () => Move(Vector3.forward),
                description = "向前移动",
                priority = 1
            },
            new KeyBind
            {
                key = KeyCode.S,
                requireCtrl = true,
                requireShift = false,
                requireAlt = false,
                onTriggered = () => Move(Vector3.back),
                description = "向后移动",
                priority = 0
            },
            new KeyBind
            {
                key = KeyCode.Space,
                requireShift = true,
                onTriggered = Jump,
                description = "超级跳",
                priority = 2
            }
        };
    }
    
    void Move(Vector3 dir)
    {
        transform.Translate(dir * (moveSpeed * Time.deltaTime));
        Debug.Log(transform.position.ToString());
    }

    void Jump()
    {
        Debug.Log("触发超级跳！");
    }

    // void OnEnable()
    // {
    //     InputManager.Instance.RegisterBinds(GetKeyBinds());
    // }
    //
    // void OnDisable()
    // {
    //     InputManager.Instance.UnregisterBinds(b => b.onTriggered == Jump); // 精确移除
    // }
    void OnEnable() => InputEventBus.RegisterBinds(GetKeyBinds());
    void OnDisable() => InputEventBus.UnregisterBinds(b => b.onTriggered == Jump);
}
