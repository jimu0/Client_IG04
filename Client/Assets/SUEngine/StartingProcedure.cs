using UnityEngine;

/// <summary>
/// 启动程序
/// </summary>
public class StartingProcedure : MonoBehaviour
{
    void Start()
    {
        // 模拟游戏初始化，比如检查更新
        Invoke("LoadLoginScene", 2.0f); // 2秒后跳转到登录场景
    }
    void LoadLoginScene()
    {
        GameStateManager.Instance.LoadScene("SceneMain");
    }
}
