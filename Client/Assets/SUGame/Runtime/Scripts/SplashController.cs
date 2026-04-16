using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

/// <summary>
/// 启动控制器 - 最先运行的程序，负责游戏启动流程和资源预加载，类似于拆掉游戏包装盒
/// 检查一些电子游戏前期的必要元素(如主场景、镜头、资源)后通过，并结束本程序使命。
/// </summary>
public class SplashController : MonoBehaviour
{
    
    [Header("启动设置")] 
    [SerializeField] private float minSplashTime = 2f; // 最小启动时间
    [SerializeField] private string  SceneMainName = "SceneMain"; 
    [SerializeField] private bool skipResourceLoading = false; // 是否跳过资源加载（用于快速测试）
    
    [Header("相机设置")] 
    [SerializeField] private string cameraPrefabPath = "CameraRoot"; // 主相机预制体路径
    public GameObject cameraRoot;

    private bool isGlobalSceneLoaded = false; //全局Scene是否已加载
    private bool isCameraLoaded = false; //主相机是否已加载
    private bool isCanvasLoaded = false; //GUICanvas是否已加载
    private bool isGameObjRootLoaded = false; //主游戏实例根是否已加载
    private bool isResourceLoadingComplete = false; // 资源加载是否完成
    private float splashStartTime; //用于判断最小启动时间是否已满足
    private bool isTriggered = true; // 用于监听是否已经触发过启动器
    private Coroutine _initCoroutine;
    

    void Start()
    {
        splashStartTime = Time.time;
        // 启动一个总的初始化流程协程，内部按顺序执行
        _initCoroutine = StartCoroutine(InitializationFlow());
    }

    private IEnumerator InitializationFlow()
    {
            
        // 1. 先初始化资源系统（YooAsset、Package 等）
        yield return ResourceManager.Init(); // 等待初始化完成

        // 2. 初始化完成，再加载全局场景
        yield return LoadGlobalScene(); // 加载主场景、相机、其他逻辑
        
        //此时游戏基本环境已准备完毕
        
        // 3. 动态创建初始游戏实例，如果有
        yield return PlaceGameObjects();


    }

    /// <summary>
    /// 加载全局场景
    /// </summary>
    private IEnumerator LoadGlobalScene()
    {
        if (SceneManager.GetActiveScene().name != SceneMainName)
        {
            GameStateManager.Instance.LoadScene(SceneMainName);
            // 等待场景完全加载完成
            yield return new WaitUntil(() => SceneManager.GetSceneByName(SceneMainName).isLoaded);
        }
        isGlobalSceneLoaded = true;
        Debug.Log("[SplashController] 全局场景加载完成");

        // 加载主镜头
        GameStateManager.Instance.LoadGlobalCamera(() => isCameraLoaded = true);
        // 加载主游戏实例根
        GameStateManager.Instance.LoadGlobalCanvas(() => isCanvasLoaded = true);
        // 加载UI组件
        GameStateManager.Instance.LoadGameObjRoot(() => isGameObjRootLoaded = true);
        // 开始bd资源加载流程
        ResourceLoadingFlow();

    }


    /// <summary>
    /// bd资源加载流程
    /// </summary>
    private void ResourceLoadingFlow()
    {
        if (skipResourceLoading)
        {
            Debug.Log("[SplashController] 跳过资源加载");
            isResourceLoadingComplete = true;
            return;
        }
        
        // 预加载Shader、Scene
        ResourceManager.PreloadRes(null);
        
        isResourceLoadingComplete = true;
    }

    private IEnumerator PlaceGameObjects()
    {
        //LoadResSync
        yield return null;
    }





    void Update()
    {
        
        if (!CanProceedToMainScene() || !isTriggered) return;

        //var packageManager = new PackageManager();
        // 示例：使用默认的本地资源包服务（也可以换成远程下载、编辑器模拟等）
        //var locator = new DefaultPackageLocator();
        //packageManager.ChangeMainPackageLocator(locator);
        
        isTriggered = false;
        StopCoroutine(_initCoroutine);
        Destroy(gameObject); // 启动程序使命结束
    }

    /// <summary>
    /// 检查是否可以开始玩游戏
    /// </summary>
    private bool CanProceedToMainScene()
    {
        // 需要满足以下条件：
        // 1. 全局场景已加载
        // 2. 全局主相机已加载
        // 3. 资源加载完成（或跳过）
        // 4. 最小启动时间已满足
        return isGlobalSceneLoaded && 
               isCameraLoaded &&
               isCanvasLoaded &&
               isGameObjRootLoaded &&
               isResourceLoadingComplete && 
               (Time.time - splashStartTime) >= minSplashTime;
    }
    
}