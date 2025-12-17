using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主界面控制器 - 负责主界面的基本功能
/// </summary>
public class MainSceneController : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Text welcomeText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        Debug.Log("[MainSceneController] 主界面初始化");
        InitializeUI();
        ShowWelcomeMessage();
    }

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    private void InitializeUI()
    {
        // 设置按钮事件
        if (startGameButton != null)
            startGameButton.onClick.AddListener(OnStartGameClick);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClick);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClick);
        
        Debug.Log("[MainSceneController] UI组件初始化完成");
    }

    /// <summary>
    /// 显示欢迎信息
    /// </summary>
    private void ShowWelcomeMessage()
    {
        // if (welcomeText != null)
        // {
        //     welcomeText.text = "欢迎来到游戏主界面！\n资源加载状态: " + (ResourceManager.Instance.AreResourcesReady() ? "已完成" : "未完成");
        // }
        
        Debug.Log("[MainSceneController] 欢迎信息显示完成");
    }

    /// <summary>
    /// 开始游戏按钮点击事件
    /// </summary>
    private void OnStartGameClick()
    {
        Debug.Log("[MainSceneController] 开始游戏");
        // 这里可以添加进入游戏逻辑
        // 比如加载游戏场景或显示游戏选项
    }

    /// <summary>
    /// 设置按钮点击事件
    /// </summary>
    private void OnSettingsClick()
    {
        Debug.Log("[MainSceneController] 打开设置");
        // 这里可以添加设置界面逻辑
    }

    /// <summary>
    /// 退出按钮点击事件
    /// </summary>
    private void OnQuitClick()
    {
        Debug.Log("[MainSceneController] 退出游戏");
        GameStateManager.Instance.QuitGame();
    }

    void OnDestroy()
    {
        // 清理按钮事件
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(OnStartGameClick);
        
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClick);
        
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClick);
    }
}
