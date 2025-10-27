using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build.Content;
using YooAsset;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// 资源管理器 - 负责游戏资源的加载和管理
/// 功能：
/// 1. 资源加载状态管理
/// 2. 模拟资源加载过程
/// 3. 资源加载进度回调
/// </summary>
public class ResourceManager
{
	
	/// <summary>
    /// 当前使用的播放模式。默认为编辑器模拟模式（EditorSimulateMode）。
    /// 可在 Inspector 中设置：
    /// - EditorSimulateMode：编辑器内模拟资源加载
    /// - OfflinePlayMode：本地资源包加载（单机）
    /// - HostPlayMode：从服务器下载资源并缓存
    /// - WebPlayMode：用于 WebGL 平台，通过 HTTP 加载资源
    /// </summary>
    public EPlayMode _playMode = EPlayMode.EditorSimulateMode;
    
    /// <summary>
    /// 默认的远程资源服务器地址（主用）
    /// </summary>
    public string defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";

    /// <summary>
    /// 备用的远程资源服务器地址（备用）
    /// </summary>
    public string fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";


    [Header("资源加载状态")]
    public bool isResourcesLoaded = false;
    public float loadingProgress = 0f;
    
    
    /// <summary>
    /// 当前使用的资源包对象，所有资源操作都基于这个包进行
    /// </summary>
    public static ResourcePackage package;

    private const string DefaultPackageName = "DefaultPackage";
    
    private static bool _isYooAssetInitialized = false;
    
    
    private static Dictionary<string, AssetOperationHandle> m_dicHandle;

    private static Dictionary<string, Shader> m_dicShaders;
    private static Dictionary<string, Object> m_dicLevelUnit;

    public static IEnumerator Init()
    {
	    m_dicHandle = new Dictionary<string, AssetOperationHandle>();
	    m_dicShaders = new Dictionary<string, Shader>();
	    m_dicLevelUnit = new Dictionary<string, Object>();
	    
	    
	    // 只在游戏启动时初始化一次 YooAsset
	    if (!_isYooAssetInitialized)
	    {
		    YooAssets.Initialize();
		    _isYooAssetInitialized = true;
	    }
	    if (!YooAssets.HasPackage(DefaultPackageName))
	    {
		    // 设置默认的资源包
		    package = YooAssets.CreatePackage(DefaultPackageName);
		    YooAssets.SetDefaultPackage(package);
#if UNITY_EDITOR
		    var initParameters = new EditorSimulateModeParameters();
		    initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(DefaultPackageName);;
		    yield return package.InitializeAsync(initParameters);
#else
        var initParameters = new OfflinePlayModeParameters();
        yield return package.InitializeAsync(initParameters);
#endif
	    }
	    else
	    {
		    package = YooAssets.GetPackage(DefaultPackageName);
	    }
	    
	    Debug.Log($"package为{ResourceManager.package!=null}");
	    

    }

    
    // /// <summary>
    // /// 单例初始化完成后的自定义初始化
    // /// </summary>
    // protected override void OnSingletonAwake()
    // {
    //     base.OnSingletonAwake();
    //     //Debug.Log("[ResourceManager] 资源管理器初始化完成");
    // }

    public static void PreloadRes(Action next)
    {
        // 加载shader
        var assetInfos = package.GetAssetInfos("Shader");
        foreach (var item in assetInfos)
        {
            var res = package.LoadAssetSync<Object>(item.Address);
            Debug.Log("Preload Shader" + item.Address);
            if (item.AssetType == typeof(Shader))
                m_dicShaders.TryAdd(item.Address, res.AssetObject as Shader);
        }

        // 加载关卡Unit
        var assetInfos2 = package.GetAssetInfos("Preload");
        foreach (var item in assetInfos2)
        {
            var res = package.LoadAssetSync<Object>(item.Address);
            Debug.Log("Preload " + item.Address);
            m_dicLevelUnit.TryAdd(item.Address, res.AssetObject);
        }
        
        next?.Invoke();
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="location"></param>
    /// <param name="callback">场景加载完成回调</param>
    /// <param name="sceneMode">主场景/子场景</param>
    /// <param name="suspendLoad">预加载场景</param>
    /// <param name="priority"></param>
    public static void LoadSceneAsync(string location, System.Action callback, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
    {
        var handler = package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
        handler.Completed += (SceneOperationHandle _handle) =>
        {
            //UnloadUnusedRes();
            callback?.Invoke();
        };
    }

    /// <summary>
    /// 同步加载资源
    /// 使用场景：小资源 或 实时性要求高的资源
    /// 注意：sprite资源使用 LoadSubRes 接口 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <returns></returns>
    public static T LoadResSync<T>(string location) where T : UnityEngine.Object
    {
        var handler = package.LoadAssetSync<T>(location);
        var res = handler.GetAssetObject<T>();
        handler.Release();
        return res;
    }

    /// <summary>
    /// 异步加载
    /// 使用场景：预加载 或 加载较大资源
    /// 注意：sprite资源使用 LoadSubRes 接口 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <param name="callback"></param>
    public static void LoadResAsync<T>(string location, System.Action<T> callback) where T : UnityEngine.Object
    {
        var handler = package.LoadAssetAsync(location);
        handler.Completed += (AssetOperationHandle _handle) =>
        {
            callback?.Invoke(_handle.GetAssetObject<T>());
            handler.Release();
        };
    }

    /// <summary>
    /// 同步加载子对象
    /// sprite
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <param name="subResName"></param>
    /// <returns></returns>
    public static T LoadSubResSync<T>(string location, string subResName) where T : UnityEngine.Object
    {
        var handler = package.LoadSubAssetsSync<T>(location);
        var res = handler.GetSubAssetObject<T>(subResName);
        handler.Release();
        return res;
    }

    /// <summary>
    /// 异步加载子对象
    /// sprite
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <param name="subResName"></param>
    /// <returns></returns>
    public static void LoadSubResAsync<T>(string location, string subResName, System.Action<T> callback) where T : UnityEngine.Object
    {
        var handler = package.LoadSubAssetsAsync<T>(location);
        handler.Completed += (SubAssetsOperationHandle _handle) =>
        {
            callback?.Invoke(_handle.GetSubAssetObject<T>(subResName));
            handler.Release();
        };
    }

    /// <summary>
    /// 同步加载bundle内所有资源
    /// 用于配置文件加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <returns></returns>
    public static T[] LoadAllAssetsSync<T>(string location) where T : UnityEngine.Object
    {
        var handler = package.LoadAllAssetsSync<T>(location);
        var res = handler.AllAssetObjects as T[];
        handler.Release();
        return res;
    }

    /// <summary>
    /// 异步加载bundle内所有资源
    /// 用于配置文件加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="location"></param>
    /// <param name="callback"></param>
    public static void LoadAllAssetsAsync<T>(string location, System.Action<T[]> callback) where T : UnityEngine.Object
    {
        var handler = package.LoadAllAssetsAsync<T>(location);
        handler.Completed += (AllAssetsOperationHandle _handle) =>
        {
            callback?.Invoke(handler.AllAssetObjects as T[]);
            handler.Release();
        };
    }

    /// <summary>
    /// 切场景调用
    /// </summary>
    public static void UnloadUnusedRes()
    {
        package.UnloadUnusedAssets();
    }
    
    
    
    
    
    //--------------------------------------------------------------------------------------------------------
    
    
    
    
    // /// <summary>
    // /// 模拟资源加载过程
    // /// </summary>
    // /// <param name="onComplete">加载完成回调</param>
    // /// <param name="onProgress">加载进度回调</param>
    // public void SimulateResourceLoading()
    // {
	   //  //package.LoadAssetSync<Object>("");
    //     //Debug.Log("[ResourceManager] 开始模拟资源加载");
    //     isResourcesLoaded = false;
    //     loadingProgress = 0f;
    //     
    //
    // }
    //
    //
    // /// <summary>
    // /// 根据不同播放模式初始化资源包
    // /// 包括请求资源版本、更新清单、下载缺失资源等步骤
    // /// </summary>
    // private IEnumerator InitPackage()
    // {
    //     InitializationOperation initOperation = null;
    //     switch (_playMode)
    //     {
	   //      case EPlayMode.EditorSimulateMode:
		  //       // 编辑器模式下使用模拟构建的资源目录
		  //       var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
		  //       var packageRoot = buildResult.PackageRootDirectory;
		  //       var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
		  //       var initParameters = new EditorSimulateModeParameters();
		  //       initParameters.EditorFileSystemParameters = editorFileSystemParams;
		  //       initOperation = package.InitializeAsync(initParameters);
		  //       break;
    //
	   //      case EPlayMode.OfflinePlayMode:
		  //       // 单机模式，使用内置资源（APK 内部或 StreamingAssets）
		  //       var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
		  //       var offinitParameters = new OfflinePlayModeParameters();
		  //       offinitParameters.BuildinFileSystemParameters = buildinFileSystemParams;
		  //       initOperation = package.InitializeAsync(offinitParameters);
		  //       break;
    //
	   //      case EPlayMode.HostPlayMode:
		  //       // 主机模式，从远程服务器下载资源，并缓存在本地
		  //       IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
		  //       var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
		  //       var hostbuildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
    //
		  //       var hostinitParameters = new HostPlayModeParameters();
		  //       hostinitParameters.BuildinFileSystemParameters = hostbuildinFileSystemParams;
		  //       hostinitParameters.CacheFileSystemParameters = cacheFileSystemParams;
		  //       initOperation = package.InitializeAsync(hostinitParameters);
		  //       break;
    //
	   //      case EPlayMode.WebPlayMode:
		  //       // Web 模式，适用于 WebGL 平台
		  //       IRemoteServices webremoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
		  //       var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
		  //       var webRemoteFileSystemParams =
			 //        FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(webremoteServices); //支持跨域下载
    //
		  //       var webinitParameters = new WebPlayModeParameters();
		  //       webinitParameters.WebServerFileSystemParameters = webServerFileSystemParams;
		  //       webinitParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;
    //
		  //       initOperation = package.InitializeAsync(webinitParameters);
		  //       break;
    //     }
    //     
    //     // 等待初始化完成
    //     yield return initOperation;
    //
    //     Debug.Log("初始化结果：" + initOperation.Status);
    //     if (initOperation.Status != EOperationStatus.Succeed)
    //     {
	   //      Debug.LogError($"初始化失败: {initOperation.Error}");
	   //      yield break;
    //     }
    //
    //     // 请求最新的资源版本信息
    //     var requestOperation = package.RequestPackageVersionAsync();
    //     Debug.Log("正在请求最新的资源版本...");
    //     yield return requestOperation;
    //
    //     string packageVersion = "";
    //     if (requestOperation.Status == EOperationStatus.Succeed)
    //     {
	   //      packageVersion = requestOperation.PackageVersion;
	   //      Debug.Log($"获取最新资源版本成功: {packageVersion}");
    //     }
    //     else
    //     {
	   //      Debug.LogError("获取资源版本失败：" + requestOperation.Error);
    //     }
    //
    //     // 更新资源清单
    //     var updateOperation = package.UpdatePackageManifestAsync(packageVersion);
    //     yield return updateOperation;
    //
    //     if (updateOperation.Status == EOperationStatus.Succeed)
    //     {
	   //      Debug.Log("资源清单更新成功");
    //     }
    //     else
    //     {
	   //      Debug.LogError("资源清单更新失败：" + updateOperation.Error);
    //     }
    //
    //     // 开始下载缺失资源
    //     yield return Download();
    // }
    //
    // /// <summary>
    // /// 下载缺失的远程资源
    // /// </summary>
    // IEnumerator Download()
    // {
	   //  int downloadingMaxNum = 10;   // 最大并发下载数
	   //  int failedTryAgain = 3;       // 下载失败重试次数
    //
	   //  var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
    //
	   //  if (downloader.TotalDownloadCount == 0)
	   //  {
		  //   Debug.Log("没有需要下载的资源");
		  //   yield return EnterGame(); // 直接进入游戏
	   //  }
    //
	   //  // 注册回调函数
	   //  downloader.DownloadFinishCallback = OnDownloadFinishFunction;
	   //  downloader.DownloadErrorCallback = OnDownloadErrorFunction;
	   //  downloader.DownloadUpdateCallback = OnDownloadUpdateFunction;
	   //  downloader.DownloadFileBeginCallback = OnDownloadFileBeginFunction;
    //
	   //  // 开始下载
	   //  downloader.BeginDownload();
	   //  yield return downloader;
    //
	   //  if (downloader.Status == EOperationStatus.Succeed)
	   //  {
		  //   Debug.Log("资源下载成功");
		  //   yield return EnterGame();
	   //  }
	   //  else
	   //  {
		  //   Debug.LogError("资源下载失败");
	   //  }
    // }
    //
    // /// <summary>
    // /// 进入游戏场景
    // /// </summary>
    // IEnumerator EnterGame()
    // {
	   //  SceneHandle handle = null;//package.LoadSceneAsync(loadSceneName);
	   //  yield return handle;
    //
	   //  if (!handle.IsDone)
	   //  {
		  //   Debug.Log($"加载场景失败：{handle.SceneObject.name}");
	   //  }
    // }
    //
    //
    // #region 下载回调函数
    //
    // /// <summary>
    // /// 下载进度更新回调
    // /// </summary>
    // private void OnDownloadUpdateFunction(DownloadUpdateData data)
    // {
	   //  float progress = (float)data.CurrentDownloadBytes / (float)data.TotalDownloadBytes;
	   //  Debug.Log($"总大小: {data.TotalDownloadBytes / 1024.0f / 1024} MB | 已下载: {data.CurrentDownloadBytes / 1024.0f / 1024} MB | 进度: {progress * 100:F2}%");
    // }
    //
    // /// <summary>
    // /// 开始下载某个文件时触发
    // /// </summary>
    // private void OnDownloadFileBeginFunction(DownloadFileData data)
    // {
	   //  Debug.Log($"开始下载文件：{data.FileName}，大小：{data.FileSize / 1024.0f} KB");
    // }
    //
    // /// <summary>
    // /// 下载错误时触发
    // /// </summary>
    // private void OnDownloadErrorFunction(DownloadErrorData data)
    // {
	   //  Debug.LogError("下载错误");
    // }
    //
    // /// <summary>
    // /// 下载结束时触发（无论成功或失败）
    // /// </summary>
    // private void OnDownloadFinishFunction(DownloaderFinishData data)
    // {
	   //  Debug.Log("下载完成");
    // }
    //
    // #endregion
    //









    /// <summary>
    /// 检查资源是否已加载
    /// </summary>
    public bool AreResourcesReady()
    {
        return isResourcesLoaded;
    }

    // /// <summary>
    // /// 单例销毁时的清理工作
    // /// </summary>
    // protected override void OnSingletonDestroy()
    // {
    //     base.OnSingletonDestroy();
    //     //Debug.Log("[ResourceManager] 资源管理器已清理");
    // }
}




internal class RemoteServices : IRemoteServices
{

	private readonly string _defaultHostServer;
	private readonly string _fallbackHostServer;

	public RemoteServices(string defaultHostServer, string fallbackHostServer)
	{
		_defaultHostServer = defaultHostServer;
		_fallbackHostServer = fallbackHostServer;
	}


	public string GetRemoteFallbackURL(string fileName)
	{
		return $"{_fallbackHostServer}/{fileName}";
	}

	public string GetRemoteMainURL(string fileName)
	{
		return $"{_defaultHostServer}/{fileName}";
	}
}
