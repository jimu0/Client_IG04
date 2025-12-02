using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [Header("FPS Text")]
    [Tooltip("可以是 TextMeshProUGUI 或 Text")]
    public TextMeshProUGUI tmpText;
    public Text uiText;
    [Header("设置")]
    public Color textColor = Color.white;
    public int fontSize = 24;

    float deltaTime = 0.0f;

    void Awake()
    {
        //QualitySettings.vSyncCount = 0;        // 关闭垂直同步
        //Application.targetFrameRate = -1;      // 不限制帧率
    }

    void Start()
    {
        // 自动找当前 GameObject 上的文本组件
        if (!tmpText) tmpText = GetComponent<TextMeshProUGUI>();
        if (!uiText) uiText = GetComponent<Text>();

        if (tmpText)
        {
            tmpText.color = textColor;
            tmpText.fontSize = fontSize;
        }
        if (uiText)
        {
            uiText.color = textColor;
            uiText.fontSize = fontSize;
        }
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        string fpsText = $"{fps:0.0} FPS";

        if (tmpText)
            tmpText.text = fpsText;
        else if (uiText)
            uiText.text = fpsText;
    }
}