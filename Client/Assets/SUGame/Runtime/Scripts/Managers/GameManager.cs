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
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        Debug.Log("[GameManager] 游戏管理器初始化完成");
        // 初始化游戏基础设置
        InitializeGame();
    }

    /// <summary>
    /// 初始化游戏基础设置
    /// </summary>
    private void InitializeGame()
    {
        // 设置目标帧率
        Application.targetFrameRate = 60;
        // 设置屏幕不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debug.Log("[GameManager] 游戏基础设置初始化完成");
    }

    /// <summary>
    /// 加载场景（同步）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[GameManager] 正在加载场景: {sceneName}");
        // 暂停游戏
        SetGamePaused(true);
        // 卸载当前场景（如果有）并加载新场景
        SceneManager.LoadScene(sceneName);
        // 恢复游戏
        SetGamePaused(false);
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="onComplete">加载完成回调</param>
    /// <param name="onProgress">加载进度回调</param>
    public IEnumerator LoadSceneAsync(string sceneName, Action onComplete = null, Action<float> onProgress = null)
    {
        Debug.Log($"[GameManager] 开始异步加载场景: {sceneName}");
        // 暂停游戏
        SetGamePaused(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        // 显示加载进度
        while (asyncLoad.progress < 0.9f)
        {
            onProgress?.Invoke(asyncLoad.progress);
            yield return null;
        }
        // 激活场景
        asyncLoad.allowSceneActivation = true;
        // 等待加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // 恢复游戏
        SetGamePaused(false);
        Debug.Log($"[GameManager] 场景加载完成: {sceneName}");
        onComplete?.Invoke();
    }

    /// <summary>
    /// 设置游戏暂停状态
    /// </summary>
    /// <param name="paused">是否暂停</param>
    public void SetGamePaused(bool paused)
    {
        isGamePaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        if (paused)
            Debug.Log("[GameManager] 游戏已暂停");
        else
            Debug.Log("[GameManager] 游戏已恢复");
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[GameManager] 退出游戏");
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