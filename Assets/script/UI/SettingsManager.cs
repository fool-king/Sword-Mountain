using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// 设置数据结构
/// </summary>
[Serializable]
public class GameSettings
{
    // 音频设置
    public float bgmVolume = 8f;
    public float voiceVolume = 8f;
    public float sfxVolume = 8f;
    
    // 视频设置
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public bool isFullscreen = true;
    
    // 游戏设置
    public float textSpeed = 5f;
    public int languageId = 0; // 0=中文, 1=英文
    
    // 控制设置
    public Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();
    public float mouseSensitivity = 1f;
    
    /// <summary>
    /// 重置为默认设置
    /// </summary>
    public void ResetToDefaults()
    {
        bgmVolume = 8f;
        voiceVolume = 8f;
        sfxVolume = 8f;
        
        // 使用当前屏幕分辨率作为默认值
        resolutionWidth = Screen.currentResolution.width;
        resolutionHeight = Screen.currentResolution.height;
        isFullscreen = true;
        
        textSpeed = 5f;
        languageId = 0;
        
        // 重置按键绑定为默认值
        InitializeDefaultKeyBindings();
        
        mouseSensitivity = 1f;
    }
    
    /// <summary>
    /// 初始化默认按键绑定
    /// </summary>
    public void InitializeDefaultKeyBindings()
    {
        keyBindings.Clear();
        keyBindings.Add("MoveForward", KeyCode.W);
        keyBindings.Add("MoveBackward", KeyCode.S);
        keyBindings.Add("MoveLeft", KeyCode.A);
        keyBindings.Add("MoveRight", KeyCode.D);
        keyBindings.Add("Jump", KeyCode.Space);
        keyBindings.Add("Interact", KeyCode.E);
        keyBindings.Add("Attack", KeyCode.Mouse0);
        keyBindings.Add("Defend", KeyCode.Mouse1);
        keyBindings.Add("Inventory", KeyCode.I);
        keyBindings.Add("SkillMenu", KeyCode.K);
        keyBindings.Add("Map", KeyCode.M);
        keyBindings.Add("Pause", KeyCode.Escape);
    }
}

/// <summary>
/// 设置管理器
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    
    private GameSettings currentSettings; // 当前设置
    private GameSettings defaultSettings; // 默认设置
    
    private const string SETTINGS_FILE_NAME = "game_settings.json";
    private const string SETTINGS_FOLDER = "/Settings/";
    
    private string settingsFilePath;
    
    public event System.Action OnSettingsChanged; // 设置变更事件
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化设置管理器
    /// </summary>
    private void Initialize()
    {
        // 初始化设置文件路径
        settingsFilePath = Application.persistentDataPath + SETTINGS_FOLDER + SETTINGS_FILE_NAME;
        
        // 确保设置文件夹存在
        string settingsFolderPath = Application.persistentDataPath + SETTINGS_FOLDER;
        if (!Directory.Exists(settingsFolderPath))
        {
            Directory.CreateDirectory(settingsFolderPath);
        }
        
        // 初始化默认设置
        defaultSettings = new GameSettings();
        defaultSettings.ResetToDefaults();
        
        // 加载设置
        LoadSettings();
        
        // 应用设置
        ApplyAllSettings();
    }
    
    /// <summary>
    /// 获取当前设置
    /// </summary>
    /// <returns>当前游戏设置</returns>
    public GameSettings GetCurrentSettings()
    {
        return currentSettings;
    }
    
    /// <summary>
    /// 加载设置
    /// </summary>
    public void LoadSettings()
    {
        try
        {
            if (File.Exists(settingsFilePath))
            {
                // 读取JSON文件
                string jsonData = File.ReadAllText(settingsFilePath);
                
                // 从JSON反序列化
                currentSettings = JsonUtility.FromJson<GameSettings>(jsonData);
                
                // 如果按键绑定为空，初始化默认值
                if (currentSettings.keyBindings == null || currentSettings.keyBindings.Count == 0)
                {
                    currentSettings.InitializeDefaultKeyBindings();
                }
                
                Debug.Log("Settings loaded successfully.");
            }
            else
            {
                // 文件不存在，使用默认设置
                currentSettings = new GameSettings();
                currentSettings.ResetToDefaults();
                SaveSettings(); // 保存默认设置
                
                Debug.Log("No settings file found, using default settings.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load settings: " + e.Message);
            
            // 加载失败时使用默认设置
            currentSettings = new GameSettings();
            currentSettings.ResetToDefaults();
        }
    }
    
    /// <summary>
    /// 保存设置
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            // 序列化设置为JSON
            string jsonData = JsonUtility.ToJson(currentSettings, true);
            
            // 写入文件
            File.WriteAllText(settingsFilePath, jsonData);
            
            Debug.Log("Settings saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save settings: " + e.Message);
        }
    }
    
    /// <summary>
    /// 应用所有设置
    /// </summary>
    public void ApplyAllSettings()
    {
        ApplyVideoSettings();
        ApplyAudioSettings();
        ApplyGameSettings();
        
        // 触发设置变更事件
        if (OnSettingsChanged != null)
        {
            OnSettingsChanged.Invoke();
        }
    }
    
    /// <summary>
    /// 应用视频设置
    /// </summary>
    public void ApplyVideoSettings()
    {
        try
        {
            // 应用分辨率和窗口模式
            Screen.SetResolution(currentSettings.resolutionWidth, currentSettings.resolutionHeight, currentSettings.isFullscreen);
            
            Debug.Log("Video settings applied: " + currentSettings.resolutionWidth + "x" + 
                      currentSettings.resolutionHeight + (currentSettings.isFullscreen ? " (Fullscreen)" : " (Windowed)"));
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to apply video settings: " + e.Message);
        }
    }
    
    /// <summary>
    /// 应用音频设置
    /// </summary>
    public void ApplyAudioSettings()
    {
        try
        {
            // 将0-10范围的音量转换为0-1范围
            float bgmVolumeNormalized = currentSettings.bgmVolume / 10f;
            float voiceVolumeNormalized = currentSettings.voiceVolume / 10f;
            float sfxVolumeNormalized = currentSettings.sfxVolume / 10f;
            
            // 这里可以调用音频管理器来应用音量设置
            // AudioManager.Instance.SetBGMVolume(bgmVolumeNormalized);
            // AudioManager.Instance.SetVoiceVolume(voiceVolumeNormalized);
            // AudioManager.Instance.SetSFXVolume(sfxVolumeNormalized);
            
            Debug.Log("Audio settings applied: BGM=" + currentSettings.bgmVolume + ", Voice=" + 
                      currentSettings.voiceVolume + ", SFX=" + currentSettings.sfxVolume);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to apply audio settings: " + e.Message);
        }
    }
    
    /// <summary>
    /// 应用游戏设置
    /// </summary>
    public void ApplyGameSettings()
    {
        try
        {
            // 文本速度设置
            // DialogueManager.Instance.SetTextSpeed(currentSettings.textSpeed);
            
            // 语言设置
            // LocalizationManager.Instance.SetLanguage(currentSettings.languageId);
            
            // 鼠标灵敏度设置
            // InputManager.Instance.SetMouseSensitivity(currentSettings.mouseSensitivity);
            
            Debug.Log("Game settings applied: TextSpeed=" + currentSettings.textSpeed + ", LanguageId=" + 
                      currentSettings.languageId + ", MouseSensitivity=" + currentSettings.mouseSensitivity);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to apply game settings: " + e.Message);
        }
    }
    
    /// <summary>
    /// 设置BGM音量
    /// </summary>
    /// <param name="volume">音量值(0-10)</param>
    public void SetBGMVolume(float volume)
    {
        currentSettings.bgmVolume = Mathf.Clamp(volume, 0f, 10f);
        ApplyAudioSettings();
    }
    
    /// <summary>
    /// 设置语音音量
    /// </summary>
    /// <param name="volume">音量值(0-10)</param>
    public void SetVoiceVolume(float volume)
    {
        currentSettings.voiceVolume = Mathf.Clamp(volume, 0f, 10f);
        ApplyAudioSettings();
    }
    
    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量值(0-10)</param>
    public void SetSFXVolume(float volume)
    {
        currentSettings.sfxVolume = Mathf.Clamp(volume, 0f, 10f);
        ApplyAudioSettings();
    }
    
    /// <summary>
    /// 设置文本速度
    /// </summary>
    /// <param name="speed">速度值(1-10)</param>
    public void SetTextSpeed(float speed)
    {
        currentSettings.textSpeed = Mathf.Clamp(speed, 1f, 10f);
        ApplyGameSettings();
    }
    
    /// <summary>
    /// 设置分辨率
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    public void SetResolution(int width, int height)
    {
        currentSettings.resolutionWidth = width;
        currentSettings.resolutionHeight = height;
        ApplyVideoSettings();
    }
    
    /// <summary>
    /// 设置窗口模式
    /// </summary>
    /// <param name="isFullscreen">是否全屏</param>
    public void SetWindowMode(bool isFullscreen)
    {
        currentSettings.isFullscreen = isFullscreen;
        ApplyVideoSettings();
    }
    
    /// <summary>
    /// 获取按键绑定
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>按键代码</returns>
    public KeyCode GetKeyBinding(string actionName)
    {
        if (currentSettings.keyBindings.ContainsKey(actionName))
        {
            return currentSettings.keyBindings[actionName];
        }
        
        // 如果找不到对应的绑定，返回默认值
        if (defaultSettings.keyBindings.ContainsKey(actionName))
        {
            return defaultSettings.keyBindings[actionName];
        }
        
        // 默认返回None
        return KeyCode.None;
    }
    
    /// <summary>
    /// 设置按键绑定
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <param name="keyCode">按键代码</param>
    public void SetKeyBinding(string actionName, KeyCode keyCode)
    {
        currentSettings.keyBindings[actionName] = keyCode;
        ApplyGameSettings();
    }
    
    /// <summary>
    /// 重置所有设置为默认值
    /// </summary>
    public void ResetAllSettings()
    {
        currentSettings.ResetToDefaults();
        ApplyAllSettings();
        SaveSettings();
    }
    
    /// <summary>
    /// 检查按键是否被按下
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>按键是否被按下</returns>
    public bool IsActionPressed(string actionName)
    {
        KeyCode keyCode = GetKeyBinding(actionName);
        if (keyCode != KeyCode.None)
        {
            return Input.GetKey(keyCode);
        }
        return false;
    }
    
    /// <summary>
    /// 检查按键是否被按下(单次触发)
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>按键是否被按下</returns>
    public bool IsActionPressedDown(string actionName)
    {
        KeyCode keyCode = GetKeyBinding(actionName);
        if (keyCode != KeyCode.None)
        {
            return Input.GetKeyDown(keyCode);
        }
        return false;
    }
}