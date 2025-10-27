using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

/// <summary>
/// 音频管理器 - 基于FMOD的专业音频管理系统
/// 功能：
/// 1. 背景音乐播放控制
/// 2. 音效播放和管理
/// 3. 音量设置和混音控制
/// 4. 音频事件的统一管理
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [Header("音频设置")]
    [Range(0f, 1f)]
    public float masterVolume = 1.0f;
    [Range(0f, 1f)]
    public float musicVolume = 0.8f;
    [Range(0f, 1f)]
    public float sfxVolume = 1.0f;
    
    [Header("FMOD事件路径")]
    public string bgmEventPath = "event:/Music/BGM";
    public string buttonClickEventPath = "event:/SFX/UI/ButtonClick";
    
    // FMOD事件实例
    private FMOD.Studio.EventInstance bgmInstance;
    private Dictionary<string, FMOD.Studio.EventInstance> loopingSounds = new Dictionary<string, FMOD.Studio.EventInstance>();
    
    // VCA控制器（音量控制）
    private FMOD.Studio.VCA masterVCA;
    private FMOD.Studio.VCA musicVCA;
    private FMOD.Studio.VCA sfxVCA;
    
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        InitializeFMOD();
        Debug.Log("[AudioManager] 音频管理器初始化完成");
    }
    
    /// <summary>
    /// 初始化FMOD系统
    /// </summary>
    private void InitializeFMOD()
    {
        try
        {
            // 获取VCA控制器
            masterVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
            musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
            sfxVCA = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");
            
            // 应用初始音量设置
            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
            
            Debug.Log("[AudioManager] FMOD系统初始化成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AudioManager] FMOD初始化失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="eventPath">FMOD事件路径</param>
    public void PlayBGM(string eventPath = "")
    {
        if (string.IsNullOrEmpty(eventPath))
            eventPath = bgmEventPath;
            
        try
        {
            // 停止当前背景音乐
            StopBGM();
            
            // 创建新的背景音乐实例
            bgmInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
            bgmInstance.start();
            
            Debug.Log($"[AudioManager] 开始播放背景音乐: {eventPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AudioManager] 播放背景音乐失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            bgmInstance.release();
            Debug.Log("[AudioManager] 停止背景音乐");
        }
    }
    
    /// <summary>
    /// 暂停/恢复背景音乐
    /// </summary>
    /// <param name="paused">是否暂停</param>
    public void SetBGMPaused(bool paused)
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.setPaused(paused);
            Debug.Log($"[AudioManager] 背景音乐{(paused ? "暂停" : "恢复")}");
        }
    }
    
    /// <summary>
    /// 播放一次性音效
    /// </summary>
    /// <param name="eventPath">FMOD事件路径</param>
    /// <param name="position">3D音效位置（可选）</param>
    public void PlaySFX(string eventPath, Vector3? position = null)
    {
        try
        {
            var eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
            
            if (position.HasValue)
            {
                eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position.Value));
            }
            
            eventInstance.start();
            eventInstance.release(); // 播放完毕后自动释放
            
            Debug.Log($"[AudioManager] 播放音效: {eventPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AudioManager] 播放音效失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 播放按钮点击音效
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickEventPath);
    }
    
    /// <summary>
    /// 设置主音量
    /// </summary>
    /// <param name="volume">音量值（0-1）</param>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (masterVCA.isValid())
        {
            masterVCA.setVolume(masterVolume);
        }
        
        // 触发音量变化事件
        if (EventSystem.HasInstance)
        {
            EventSystem.Instance.Trigger(GameEvent.AUDIO_VOLUME_CHANGED, new { type = "master", volume = masterVolume });
        }
        
        Debug.Log($"[AudioManager] 设置主音量: {masterVolume:F2}");
    }
    
    /// <summary>
    /// 设置音乐音量
    /// </summary>
    /// <param name="volume">音量值（0-1）</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicVCA.isValid())
        {
            musicVCA.setVolume(musicVolume);
        }
        
        Debug.Log($"[AudioManager] 设置音乐音量: {musicVolume:F2}");
    }
    
    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量值（0-1）</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxVCA.isValid())
        {
            sfxVCA.setVolume(sfxVolume);
        }
        
        Debug.Log($"[AudioManager] 设置音效音量: {sfxVolume:F2}");
    }
    
    /// <summary>
    /// 静音/取消静音
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void SetMute(bool mute)
    {
        float volume = mute ? 0f : masterVolume;
        if (masterVCA.isValid())
        {
            masterVCA.setVolume(volume);
        }
        
        Debug.Log($"[AudioManager] {(mute ? "静音" : "取消静音")}");
    }
    
    /// <summary>
    /// 暂停所有音频
    /// </summary>
    /// <param name="paused">是否暂停</param>
    public void SetAllAudioPaused(bool paused)
    {
        FMODUnity.RuntimeManager.PauseAllEvents(paused);
        Debug.Log($"[AudioManager] {(paused ? "暂停" : "恢复")}所有音频");
    }
    
    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        // 停止所有音频
        StopBGM();
        
        // 清理循环音效
        foreach (var kvp in loopingSounds)
        {
            if (kvp.Value.isValid())
            {
                kvp.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                kvp.Value.release();
            }
        }
        loopingSounds.Clear();
        
        base.OnSingletonDestroy();
        Debug.Log("[AudioManager] 音频管理器已清理");
    }
}
