using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private TouchControls touchControls;
    
    public Vector2 moveValue;
    
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        Debug.Log("[InputManager] 输入管理器初始化完成");
    }

    void Awake()
    {
        touchControls = new TouchControls();
    }

    void Start()
    {
        
    }

    void Update()
    {
        moveValue = touchControls.Touch.Move.ReadValue<Vector2>();
    }
    
    public Vector3 UpdateDir()
    {
        return new Vector3(moveValue.x, 0, moveValue.y);
    }
    
    
    public void OnEnable()
    {
        touchControls.Enable();
    }
    public void OnDisable()
    {
        touchControls.Disable();
    }
    private void OnDestroy()
    {
        // 销毁时取消注册，避免空引用或错误调用
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        base.OnSingletonDestroy();
        Destroy(gameObject);
        Debug.Log("[TouchInputManager] 触摸输入系统已清理");
    }
}
