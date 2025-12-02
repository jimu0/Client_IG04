using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// 游戏主管理器 - 负责游戏的整体流程控制
/// 功能：
/// 1. 场景管理和切换
/// 2. 游戏状态管理
/// 3. 全局事件协调
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("游戏状态")]
    public bool isGamePaused = false;

    private const string CameraPrefabPath = "CameraRoot"; // 主相机预制体路径
    public GameObject cameraRoot;//主镜头
    
    public GameMod gameMod;//游戏模式

    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        //Debug.Log("[GameManager] 游戏管理器初始化完成");
        // 初始化游戏基础设置
        InitializeGame();
    }
    

    /// <summary>
    /// 初始化游戏基础设置
    /// </summary>
    private void InitializeGame()
    {
        // 关闭垂直同步
        QualitySettings.vSyncCount = 0;
        // 设置目标帧率
        Application.targetFrameRate = 280;
        // 安卓使用最高支持刷新率
        //Application.targetFrameRate = Screen.currentResolution.refreshRate;
        // 设置屏幕不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /// <summary>
    /// 加载全局主相机
    /// </summary>
    public void LoadGlobalCamera(Action isCameraLoaded)
    {
        if (cameraRoot != null) return;
        GameObject cameraPrefab = Resources.Load<GameObject>(CameraPrefabPath); // 从Resources文件夹加载相机预制体
        if (cameraPrefab == null)
        {
            Debug.LogError($"[SplashController] 无法加载相机预制体: {CameraPrefabPath}");
            return;
        }
        cameraRoot = Instantiate(cameraPrefab); // 实例化相机预制体
        DontDestroyOnLoad(cameraRoot); // 设置为DontDestroyOnLoad，确保在所有场景中保持
        isCameraLoaded.Invoke();
    }
    
    /// <summary>
    /// 加载场景（同步）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void LoadScene(string sceneName)
    {
        SetGamePaused(true);// 暂停游戏
        // 卸载当前场景（如果有）并加载新场景
        SceneManager.LoadScene(sceneName);
        SetGamePaused(false);// 恢复游戏
    }

    

    public void GenerateGameMod()
    {
        gameMod = gameObject.AddComponent<GameMod>();
        //DontDestroyOnLoad(gameMod);
    }


    /// <summary>
    /// 设置游戏暂停状态
    /// </summary>
    /// <param name="paused">是否暂停</param>
    public void SetGamePaused(bool paused)
    {
        isGamePaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        // if (paused)
        //     Debug.Log("[GameManager] 游戏已暂停");
        // else
        //     Debug.Log("[GameManager] 游戏已恢复");
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        //Debug.Log("[GameManager] 退出游戏");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        base.OnSingletonDestroy();
        Debug.Log("[GameManager] 游戏管理器已清理");
    }
}

