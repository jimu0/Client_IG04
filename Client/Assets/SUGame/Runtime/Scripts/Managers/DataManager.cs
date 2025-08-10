using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 数据管理器 - 负责游戏数据的持久化存储和管理
/// 功能：
/// 1. 游戏设置数据保存/加载
/// 2. 玩家进度数据管理
/// 3. 配置文件的读写
/// 4. 数据的序列化和反序列化
/// </summary>
public class DataManager : Singleton<DataManager>
{
    [Header("数据存储设置")]
    public bool useEncryption = false; // 是否使用加密存储
    public string saveFileName = "SaveData.json";
    public string settingsFileName = "Settings.json";
    
    private string saveDirectory;
    private Dictionary<string, object> runtimeData = new Dictionary<string, object>();
    
    /// <summary>
    /// 游戏设置数据结构
    /// </summary>
    [System.Serializable]
    public class GameSettings
    {
        public float masterVolume = 1.0f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1.0f;
        public int graphicsQuality = 2; // 0=低, 1=中, 2=高
        public bool fullScreen = true;
        public int resolutionWidth = 1920;
        public int resolutionHeight = 1080;
        public string language = "zh-CN";
        
        public GameSettings()
        {
            // 构造函数，使用默认值
        }
    }
    
    /// <summary>
    /// 玩家存档数据结构
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public string playerName = "";
        public int level = 1;
        public int experience = 0;
        public int coins = 0;
        public DateTime lastSaveTime;
        public Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();
        public Dictionary<string, int> gameProgress = new Dictionary<string, int>();
        
        public SaveData()
        {
            lastSaveTime = DateTime.Now;
        }
    }
    
    private GameSettings currentSettings;
    private SaveData currentSaveData;
    
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        InitializeDataSystem();
        Debug.Log("[DataManager] 数据管理器初始化完成");
    }
    
    /// <summary>
    /// 初始化数据系统
    /// </summary>
    private void InitializeDataSystem()
    {
        // 设置存档目录
        saveDirectory = Path.Combine(Application.persistentDataPath, "SaveData");
        
        // 确保存档目录存在
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
            Debug.Log($"[DataManager] 创建存档目录: {saveDirectory}");
        }
        
        // 加载游戏设置
        LoadSettings();
        
        // 应用设置到游戏
        ApplySettings();
        
        Debug.Log($"[DataManager] 数据存储目录: {saveDirectory}");
    }
    
    #region 设置数据管理
    
    /// <summary>
    /// 加载游戏设置
    /// </summary>
    public void LoadSettings()
    {
        string filePath = Path.Combine(saveDirectory, settingsFileName);
        
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                currentSettings = JsonUtility.FromJson<GameSettings>(json);
                Debug.Log("[DataManager] 游戏设置加载成功");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] 加载设置失败: {e.Message}");
                currentSettings = new GameSettings(); // 使用默认设置
            }
        }
        else
        {
            currentSettings = new GameSettings(); // 使用默认设置
            SaveSettings(); // 创建默认设置文件
            Debug.Log("[DataManager] 创建默认游戏设置");
        }
    }
    
    /// <summary>
    /// 保存游戏设置
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            string filePath = Path.Combine(saveDirectory, settingsFileName);
            string json = JsonUtility.ToJson(currentSettings, true);
            File.WriteAllText(filePath, json);
            Debug.Log("[DataManager] 游戏设置保存成功");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 保存设置失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 应用设置到游戏系统
    /// </summary>
    private void ApplySettings()
    {
        // 应用音频设置
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.SetMasterVolume(currentSettings.masterVolume);
            AudioManager.Instance.SetMusicVolume(currentSettings.musicVolume);
            AudioManager.Instance.SetSFXVolume(currentSettings.sfxVolume);
        }
        
        // 应用图形设置
        QualitySettings.SetQualityLevel(currentSettings.graphicsQuality);
        Screen.fullScreen = currentSettings.fullScreen;
        Screen.SetResolution(currentSettings.resolutionWidth, currentSettings.resolutionHeight, currentSettings.fullScreen);
        
        Debug.Log("[DataManager] 游戏设置应用完成");
    }
    
    /// <summary>
    /// 获取当前游戏设置
    /// </summary>
    /// <returns>游戏设置对象</returns>
    public GameSettings GetSettings()
    {
        return currentSettings;
    }
    
    /// <summary>
    /// 更新设置并保存
    /// </summary>
    /// <param name="newSettings">新的设置</param>
    public void UpdateSettings(GameSettings newSettings)
    {
        currentSettings = newSettings;
        SaveSettings();
        ApplySettings();
    }
    
    #endregion
    
    #region 存档数据管理
    
    /// <summary>
    /// 加载存档数据
    /// </summary>
    /// <param name="slotIndex">存档槽位（0-2）</param>
    /// <returns>是否加载成功</returns>
    public bool LoadSaveData(int slotIndex = 0)
    {
        string fileName = $"Save_{slotIndex}_{saveFileName}";
        string filePath = Path.Combine(saveDirectory, fileName);
        
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                currentSaveData = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"[DataManager] 存档数据加载成功，槽位: {slotIndex}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] 加载存档失败: {e.Message}");
                return false;
            }
        }
        else
        {
            currentSaveData = new SaveData(); // 创建新存档
            Debug.Log($"[DataManager] 创建新存档，槽位: {slotIndex}");
            return false;
        }
    }
    
    /// <summary>
    /// 保存存档数据
    /// </summary>
    /// <param name="slotIndex">存档槽位（0-2）</param>
    /// <returns>是否保存成功</returns>
    public bool SaveGameData(int slotIndex = 0)
    {
        try
        {
            if (currentSaveData == null)
                currentSaveData = new SaveData();
                
            currentSaveData.lastSaveTime = DateTime.Now;
            
            string fileName = $"Save_{slotIndex}_{saveFileName}";
            string filePath = Path.Combine(saveDirectory, fileName);
            string json = JsonUtility.ToJson(currentSaveData, true);
            
            File.WriteAllText(filePath, json);
            Debug.Log($"[DataManager] 存档数据保存成功，槽位: {slotIndex}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 保存存档失败: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 获取当前存档数据
    /// </summary>
    /// <returns>存档数据对象</returns>
    public SaveData GetSaveData()
    {
        if (currentSaveData == null)
            currentSaveData = new SaveData();
        return currentSaveData;
    }
    
    /// <summary>
    /// 检查存档是否存在
    /// </summary>
    /// <param name="slotIndex">存档槽位</param>
    /// <returns>是否存在</returns>
    public bool SaveExists(int slotIndex)
    {
        string fileName = $"Save_{slotIndex}_{saveFileName}";
        string filePath = Path.Combine(saveDirectory, fileName);
        return File.Exists(filePath);
    }
    
    /// <summary>
    /// 删除存档
    /// </summary>
    /// <param name="slotIndex">存档槽位</param>
    /// <returns>是否删除成功</returns>
    public bool DeleteSave(int slotIndex)
    {
        try
        {
            string fileName = $"Save_{slotIndex}_{saveFileName}";
            string filePath = Path.Combine(saveDirectory, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"[DataManager] 删除存档成功，槽位: {slotIndex}");
                return true;
            }
            
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataManager] 删除存档失败: {e.Message}");
            return false;
        }
    }
    
    #endregion
    
    #region 运行时数据管理
    
    /// <summary>
    /// 设置运行时数据
    /// </summary>
    /// <param name="key">数据键</param>
    /// <param name="value">数据值</param>
    public void SetRuntimeData(string key, object value)
    {
        runtimeData[key] = value;
        Debug.Log($"[DataManager] 设置运行时数据: {key}");
    }
    
    /// <summary>
    /// 获取运行时数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">数据键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>数据值</returns>
    public T GetRuntimeData<T>(string key, T defaultValue = default(T))
    {
        if (runtimeData.ContainsKey(key))
        {
            try
            {
                return (T)runtimeData[key];
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] 获取运行时数据类型转换失败: {e.Message}");
                return defaultValue;
            }
        }
        
        return defaultValue;
    }
    
    /// <summary>
    /// 清除所有运行时数据
    /// </summary>
    public void ClearRuntimeData()
    {
        int count = runtimeData.Count;
        runtimeData.Clear();
        Debug.Log($"[DataManager] 清除了 {count} 个运行时数据");
    }
    
    #endregion
    
    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        // 自动保存当前设置
        if (currentSettings != null)
        {
            SaveSettings();
        }
        
        // 清理运行时数据
        ClearRuntimeData();
        
        base.OnSingletonDestroy();
        Debug.Log("[DataManager] 数据管理器已清理");
    }
}
