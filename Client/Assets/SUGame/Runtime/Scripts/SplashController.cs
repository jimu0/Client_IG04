using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    void Start()
    {
        // 先加载全局场景，但要注意不要重复加载
        // 判断是否已经存在GameManager
        if (GameManager.Instance == null)
        {
            SceneManager.LoadScene("Global", LoadSceneMode.Additive);
        }

        // 等待2秒后跳转到登录场景
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(2);
        // 卸载启动场景，加载登录场景
        // 注意：这里我们使用Single模式加载登录场景，这样会卸载启动场景，但Global场景因为是Additive且标记了DontDestroyOnLoad，所以会保留
        SceneManager.LoadScene("LoginScene");
    }
}
