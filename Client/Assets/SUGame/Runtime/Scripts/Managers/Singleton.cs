using UnityEngine;

/// <summary>
/// 通用单例基类 - 继承MonoBehaviour的单例模式
/// 特点：
/// 1. 线程安全
/// 2. 自动DontDestroyOnLoad
/// 3. 场景切换时保持唯一性
/// 4. 支持懒加载
/// </summary>
/// <typeparam name="T">继承此基类的具体管理器类型</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance
    {
        get
        {
            // 应用程序退出时不再创建新实例
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] 应用程序正在退出，无法访问 {typeof(T)} 实例。");
                return null;
            }

            // 线程安全的单例获取
            lock (_lock)
            {
                if (_instance == null)
                {
                    // 在场景中查找现有实例
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        // 创建新的GameObject并添加组件
                        GameObject singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
                        _instance = singletonObject.AddComponent<T>();
                        // 标记为不销毁
                        DontDestroyOnLoad(singletonObject);
                        Debug.Log($"[Singleton] 创建了新的 {typeof(T).Name} 实例");
                    }
                    else
                    {
                        // 确保现有实例也标记为不销毁
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// 检查实例是否存在（不会创建新实例）
    /// </summary>
    public static bool HasInstance => _instance != null;

    /// <summary>
    /// 在Awake中进行单例初始化
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnSingletonAwake();
        }
        else if (_instance != this)
        {
            // 如果已经存在其他实例，销毁当前对象
            Debug.LogWarning($"[Singleton] 发现重复的 {typeof(T).Name} 实例，销毁多余实例");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 单例初始化完成后调用，子类可以重写此方法进行初始化
    /// </summary>
    protected virtual void OnSingletonAwake() { }

    /// <summary>
    /// 应用程序退出时的清理
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    /// <summary>
    /// 销毁时的清理
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
        OnSingletonDestroy();
    }

    /// <summary>
    /// 单例销毁时调用，子类可以重写此方法进行清理
    /// </summary>
    protected virtual void OnSingletonDestroy() { }
}
