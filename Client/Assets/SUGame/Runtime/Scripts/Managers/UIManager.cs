using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// UI管理器 - 负责UI界面的显示、隐藏和层级管理
/// 功能：
/// 1. UI面板的堆栈管理
/// 2. UI动画和过渡效果
/// 3. 加载进度界面管理
/// 4. UI资源的统一管理
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [Header("UI设置")]
    public Canvas mainCanvas;
    public GameObject loadingPanel;
    public UnityEngine.UI.Slider loadingBar;
    public UnityEngine.UI.Text loadingText;
    
    private Stack<GameObject> _uiStack = new Stack<GameObject>();
    private Dictionary<string, GameObject> _cachedPanels = new Dictionary<string, GameObject>();
    
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        
        // 查找主画布
        if (mainCanvas == null)
            mainCanvas = FindObjectOfType<Canvas>();
            
        Debug.Log("[UIManager] UI管理器初始化完成");
    }
    
    /// <summary>
    /// 推入UI面板到堆栈顶部
    /// </summary>
    /// <param name="panel">要显示的UI面板</param>
    /// <param name="animate">是否使用动画</param>
    public void PushUI(GameObject panel, bool animate = true)
    {
        if (panel == null)
        {
            Debug.LogWarning("[UIManager] 尝试推入空的UI面板");
            return;
        }
        
        // 隐藏当前顶部面板
        if (_uiStack.Count > 0)
        {
            GameObject currentTop = _uiStack.Peek();
            if (animate)
                HideUIWithAnimation(currentTop);
            else
                currentTop.SetActive(false);
        }
        
        // 显示新面板
        _uiStack.Push(panel);
        if (animate)
            ShowUIWithAnimation(panel);
        else
            panel.SetActive(true);
            
        Debug.Log($"[UIManager] 推入UI面板: {panel.name}，当前堆栈深度: {_uiStack.Count}");
        
        // 触发UI切换事件
        EventSystem.Instance.Trigger("UI_PANEL_PUSHED", panel.name);
    }
    
    /// <summary>
    /// 弹出堆栈顶部的UI面板
    /// </summary>
    /// <param name="animate">是否使用动画</param>
    public void PopUI(bool animate = true)
    {
        if (_uiStack.Count == 0)
        {
            Debug.LogWarning("[UIManager] UI堆栈为空，无法弹出");
            return;
        }
        
        GameObject top = _uiStack.Pop();
        
        // 隐藏顶部面板
        if (animate)
            HideUIWithAnimation(top);
        else
            top.SetActive(false);
            
        // 显示新的顶部面板
        if (_uiStack.Count > 0)
        {
            GameObject newTop = _uiStack.Peek();
            if (animate)
                ShowUIWithAnimation(newTop);
            else
                newTop.SetActive(true);
        }
        
        Debug.Log($"[UIManager] 弹出UI面板: {top.name}，当前堆栈深度: {_uiStack.Count}");
        
        // 触发UI切换事件
        EventSystem.Instance.Trigger("UI_PANEL_POPPED", top.name);
    }
    
    /// <summary>
    /// 清空UI堆栈
    /// </summary>
    public void ClearUIStack()
    {
        while (_uiStack.Count > 0)
        {
            GameObject panel = _uiStack.Pop();
            panel.SetActive(false);
        }
        
        Debug.Log("[UIManager] 清空UI堆栈");
    }
    
    /// <summary>
    /// 显示加载界面
    /// </summary>
    /// <param name="message">加载提示信息</param>
    public void ShowLoading(string message = "正在加载...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            if (loadingText != null)
                loadingText.text = message;
            if (loadingBar != null)
                loadingBar.value = 0f;
                
            Debug.Log($"[UIManager] 显示加载界面: {message}");
        }
    }
    
    /// <summary>
    /// 更新加载进度
    /// </summary>
    /// <param name="progress">进度值（0-1）</param>
    public void UpdateLoadingProgress(float progress)
    {
        if (loadingBar != null)
        {
            loadingBar.value = progress;
            
            if (loadingText != null)
                loadingText.text = $"正在加载... {(progress * 100):F0}%";
        }
    }
    
    /// <summary>
    /// 隐藏加载界面
    /// </summary>
    public void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
            Debug.Log("[UIManager] 隐藏加载界面");
        }
    }
    
    /// <summary>
    /// 带动画显示UI
    /// </summary>
    /// <param name="panel">UI面板</param>
    private void ShowUIWithAnimation(GameObject panel)
    {
        panel.SetActive(true);
        
        // 缩放动画
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack);
        }
    }
    
    /// <summary>
    /// 带动画隐藏UI
    /// </summary>
    /// <param name="panel">UI面板</param>
    private void HideUIWithAnimation(GameObject panel)
    {
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => panel.SetActive(false));
        }
        else
        {
            panel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 获取当前UI堆栈深度
    /// </summary>
    /// <returns>堆栈深度</returns>
    public int GetUIStackDepth()
    {
        return _uiStack.Count;
    }
    
    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        ClearUIStack();
        _cachedPanels.Clear();
        base.OnSingletonDestroy();
        Debug.Log("[UIManager] UI管理器已清理");
    }
}