using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载器 - 负责场景的异步加载和切换
/// 功能：
/// 1. 异步场景加载
/// 2. 加载进度显示
/// 3. 场景切换动画
/// 4. 登录验证和跳转
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    [Header("场景加载设置")]
    [SerializeField] private float minimumLoadingTime = 1.0f; // 最小加载时间，确保用户能看到加载界面
    
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        Debug.Log("[SceneLoader] 场景加载器初始化完成");
    }
    
    /// <summary>
    /// 加载场景（叠加模式）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName, LoadSceneMode.Additive));
    }
    
    /// <summary>
    /// 加载场景（替换模式）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="showLoading">是否显示加载界面</param>
    public void LoadSceneSingle(string sceneName, bool showLoading = true)
    {
        StartCoroutine(LoadAsync(sceneName, LoadSceneMode.Single, showLoading));
    }

    /// <summary>
    /// 异步加载场景的核心逻辑
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="mode">加载模式</param>
    /// <param name="showLoading">是否显示加载界面</param>
    private IEnumerator LoadAsync(string sceneName, LoadSceneMode mode, bool showLoading = true)
    {
        float startTime = Time.realtimeSinceStartup;
        
        Debug.Log($"[SceneLoader] 开始异步加载场景: {sceneName}，模式: {mode}");
        
        // 显示加载界面
        if (showLoading && UIManager.HasInstance)
        {
            UIManager.Instance.ShowLoading($"正在加载 {sceneName}...");
        }
        
        // 开始异步加载
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
        asyncOperation.allowSceneActivation = false;
        
        // 更新加载进度
        while (asyncOperation.progress < 0.9f)
        {
            float progress = asyncOperation.progress;
            
            if (showLoading && UIManager.HasInstance)
            {
                UIManager.Instance.UpdateLoadingProgress(progress);
            }
            
            Debug.Log($"[SceneLoader] 加载进度: {progress * 100:F1}%");
            yield return null;
        }
        
        // 确保最小加载时间
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        if (elapsedTime < minimumLoadingTime)
        {
            yield return new WaitForSecondsRealtime(minimumLoadingTime - elapsedTime);
        }
        
        // 激活场景
        if (showLoading && UIManager.HasInstance)
        {
            UIManager.Instance.UpdateLoadingProgress(1.0f);
        }
        
        asyncOperation.allowSceneActivation = true;
        
        // 等待场景完全加载
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        
        // 隐藏加载界面
        if (showLoading && UIManager.HasInstance)
        {
            UIManager.Instance.HideLoading();
        }
        
        Debug.Log($"[SceneLoader] 场景加载完成: {sceneName}");
        
        // 触发场景加载完成事件
        if (EventSystem.HasInstance)
        {
            EventSystem.Instance.Trigger(GameEvent.SCENE_LOADED, sceneName);
        }
    }
    
    /// <summary>
    /// 登录场景中点击登录按钮后触发进入MainScene
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    public void OnLoginButtonClicked(string username = "", string password = "")
    {
        StartCoroutine(HandleLogin(username, password));
    }
    
    /// <summary>
    /// 处理登录逻辑
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    private IEnumerator HandleLogin(string username, string password)
    {
        Debug.Log($"[SceneLoader] 开始登录验证: {username}");
        
        // 显示登录验证界面
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowLoading("正在验证账号...");
        }
        
        // 模拟网络验证延迟
        yield return new WaitForSeconds(1.5f);
        
        // 简单的验证逻辑（实际项目中应该连接服务器）
        bool loginSuccess = ValidateLogin(username, password);
        
        if (loginSuccess)
        {
            Debug.Log("[SceneLoader] 登录验证成功");
            
            // 触发登录成功事件
            if (EventSystem.HasInstance)
            {
                EventSystem.Instance.Trigger(GameEvent.PLAYER_LOGIN, username);
            }
            
            // 跳转到主场景
            LoadSceneSingle("MainScene");
        }
        else
        {
            Debug.LogWarning("[SceneLoader] 登录验证失败");
            
            // 隐藏加载界面
            if (UIManager.HasInstance)
            {
                UIManager.Instance.HideLoading();
            }
            
            // 这里可以显示错误提示
            // UIManager.Instance.ShowErrorMessage("用户名或密码错误");
        }
    }
    
    /// <summary>
    /// 验证登录信息（示例实现）
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>验证结果</returns>
    private bool ValidateLogin(string username, string password)
    {
        // 简单的示例验证逻辑
        // 实际项目中应该与服务器进行通信验证
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // 允许空账号密码（开发阶段）
            return true;
        }
        
        // 示例：任何非空用户名和密码都能通过验证
        return username.Length > 0 && password.Length > 0;
    }
    
    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        base.OnSingletonDestroy();
        Debug.Log("[SceneLoader] 场景加载器已清理");
    }
}