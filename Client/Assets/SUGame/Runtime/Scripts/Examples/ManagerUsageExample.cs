using UnityEngine;

/// <summary>
/// 管理器使用示例 - 展示如何正确使用各种单例管理器
/// 这个脚本可以挂载到任何GameObject上进行测试
/// </summary>
public class ManagerUsageExample : MonoBehaviour
{
    [Header("测试设置")]
    public KeyCode testKey = KeyCode.T;
    
    void Start()
    {
        // 订阅一些游戏事件
        SubscribeToEvents();
        
        // 展示数据管理器的基本用法
        DemoDataManager();
        
        Debug.Log("[ManagerUsageExample] 示例脚本初始化完成，按 T 键测试功能");
    }
    
    void Update()
    {
        // 按键测试功能
        if (Input.GetKeyDown(testKey))
        {
            TestManagerFunctions();
        }
    }
    
    /// <summary>
    /// 订阅游戏事件
    /// </summary>
    private void SubscribeToEvents()
    {
        // 订阅场景加载完成事件
        EventSystem.Instance.Subscribe(GameEvent.SCENE_LOADED, OnSceneLoaded);
        
        // 订阅玩家登录事件
        EventSystem.Instance.Subscribe(GameEvent.PLAYER_LOGIN, OnPlayerLogin);
        
        // 订阅音量变化事件
        EventSystem.Instance.Subscribe(GameEvent.AUDIO_VOLUME_CHANGED, OnAudioVolumeChanged);
        
        Debug.Log("[ManagerUsageExample] 事件订阅完成");
    }
    
    /// <summary>
    /// 演示数据管理器的基本功能
    /// </summary>
    private void DemoDataManager()
    {
        // 获取当前游戏设置
        var settings = DataManager.Instance.GetSettings();
        Debug.Log($"[ManagerUsageExample] 当前音乐音量: {settings.musicVolume}");
        
        // 设置一些运行时数据
        DataManager.Instance.SetRuntimeData("demo_score", 1000);
        DataManager.Instance.SetRuntimeData("demo_level", "Level_1");
        
        // 获取运行时数据
        int score = DataManager.Instance.GetRuntimeData<int>("demo_score", 0);
        string level = DataManager.Instance.GetRuntimeData<string>("demo_level", "Unknown");
        
        Debug.Log($"[ManagerUsageExample] 运行时数据 - 分数: {score}, 关卡: {level}");
    }
    
    /// <summary>
    /// 测试各管理器的功能
    /// </summary>
    private void TestManagerFunctions()
    {
        Debug.Log("[ManagerUsageExample] 开始测试管理器功能...");
        
        // 测试音频管理器
        TestAudioManager();
        
        // 测试UI管理器
        TestUIManager();
        
        // 测试事件系统
        TestEventSystem();
        
        // 测试场景加载器
        TestSceneLoader();
    }
    
    /// <summary>
    /// 测试音频管理器
    /// </summary>
    private void TestAudioManager()
    {
        Debug.Log("[ManagerUsageExample] 测试音频管理器...");
        
        // 播放按钮点击音效
        AudioManager.Instance.PlayButtonClick();
        
        // 调整音量
        float newVolume = Random.Range(0.5f, 1.0f);
        AudioManager.Instance.SetMasterVolume(newVolume);
        
        Debug.Log($"[ManagerUsageExample] 设置新的主音量: {newVolume:F2}");
    }
    
    /// <summary>
    /// 测试UI管理器
    /// </summary>
    private void TestUIManager()
    {
        Debug.Log("[ManagerUsageExample] 测试UI管理器...");
        
        // 显示加载界面
        UIManager.Instance.ShowLoading("测试加载中...");
        
        // 2秒后隐藏
        Invoke(nameof(HideTestLoading), 2.0f);
        
        Debug.Log($"[ManagerUsageExample] UI堆栈深度: {UIManager.Instance.GetUIStackDepth()}");
    }
    
    private void HideTestLoading()
    {
        UIManager.Instance.HideLoading();
        Debug.Log("[ManagerUsageExample] 测试加载界面已隐藏");
    }
    
    /// <summary>
    /// 测试事件系统
    /// </summary>
    private void TestEventSystem()
    {
        Debug.Log("[ManagerUsageExample] 测试事件系统...");
        
        // 触发一个自定义事件
        EventSystem.Instance.Trigger("TEST_EVENT", "这是一个测试事件");
        
        // 触发关卡完成事件
        EventSystem.Instance.Trigger(GameEvent.LEVEL_COMPLETE, new { level = 1, score = 1500 });
        
        Debug.Log("[ManagerUsageExample] 事件触发完成");
    }
    
    /// <summary>
    /// 测试场景加载器
    /// </summary>
    private void TestSceneLoader()
    {
        Debug.Log("[ManagerUsageExample] 测试场景加载器...");
        
        // 注意：这里只是演示API用法，实际使用时需要确保场景存在
        // SceneLoader.Instance.LoadSceneSingle("TestScene");
        
        Debug.Log("[ManagerUsageExample] 场景加载器测试（模拟）");
    }
    
    #region 事件回调函数
    
    /// <summary>
    /// 场景加载完成回调
    /// </summary>
    /// <param name="data">事件数据</param>
    private void OnSceneLoaded(object data)
    {
        string sceneName = data as string;
        Debug.Log($"[ManagerUsageExample] 场景加载完成回调: {sceneName}");
    }
    
    /// <summary>
    /// 玩家登录完成回调
    /// </summary>
    /// <param name="data">事件数据</param>
    private void OnPlayerLogin(object data)
    {
        string username = data as string;
        Debug.Log($"[ManagerUsageExample] 玩家登录完成回调: {username}");
    }
    
    /// <summary>
    /// 音量变化回调
    /// </summary>
    /// <param name="data">事件数据</param>
    private void OnAudioVolumeChanged(object data)
    {
        Debug.Log($"[ManagerUsageExample] 音量变化回调: {data}");
    }
    
    #endregion
    
    /// <summary>
    /// 脚本销毁时取消事件订阅
    /// </summary>
    void OnDestroy()
    {
        // 取消事件订阅，防止内存泄漏
        if (EventSystem.HasInstance)
        {
            EventSystem.Instance.Unsubscribe(GameEvent.SCENE_LOADED, OnSceneLoaded);
            EventSystem.Instance.Unsubscribe(GameEvent.PLAYER_LOGIN, OnPlayerLogin);
            EventSystem.Instance.Unsubscribe(GameEvent.AUDIO_VOLUME_CHANGED, OnAudioVolumeChanged);
        }
        
        Debug.Log("[ManagerUsageExample] 事件订阅已取消");
    }
}
