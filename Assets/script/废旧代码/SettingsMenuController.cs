using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Settings Sliders")]
    public Slider textSpeedSlider;
    public Slider bgmVolumeSlider;
    public Slider voiceVolumeSlider;

    [Header("Settings Dropdowns")]
    public TMP_Dropdown windowModeDropdown;
    public TMP_Dropdown resolutionDropdown;

    [Header("Other References")]
    public Button keyConfigButton;
    public Button backButton;

    private Resolution[] resolutions;

    private void Start()
    {
        InitializeSettings();
        LoadCurrentSettings();

        // 绑定事件
        textSpeedSlider.onValueChanged.AddListener(OnTextSpeedChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        voiceVolumeSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
        windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        
        keyConfigButton.onClick.AddListener(OpenKeyConfig);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void InitializeSettings()
    {
        // 初始化文本速度滑块
        textSpeedSlider.minValue = 1;
        textSpeedSlider.maxValue = 10;
        textSpeedSlider.wholeNumbers = true;

        // 初始化音量滑块
        bgmVolumeSlider.minValue = 0;
        bgmVolumeSlider.maxValue = 10;
        bgmVolumeSlider.wholeNumbers = true;

        voiceVolumeSlider.minValue = 0;
        voiceVolumeSlider.maxValue = 10;
        voiceVolumeSlider.wholeNumbers = true;

        // 初始化窗口模式下拉菜单
        windowModeDropdown.ClearOptions();
        windowModeDropdown.AddOptions(new List<string> { "全屏", "窗口模式" });

        // 初始化分辨率下拉菜单
        InitializeResolutionDropdown();
    }

    private void InitializeResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadCurrentSettings()
    {
        // 从PlayerPrefs或游戏设置中加载当前设置
        textSpeedSlider.value = PlayerPrefs.GetFloat("TextSpeed", 5f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 8f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 8f);
        windowModeDropdown.value = PlayerPrefs.GetInt("WindowMode", 0);
    }

    // 设置变更事件处理
    private void OnTextSpeedChanged(float value)
    {
        PlayerPrefs.SetFloat("TextSpeed", value);
        // 这里可以调用对话管理器的设置文本速度方法
    }

    private void OnBGMVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("BGMVolume", value);
        // 这里可以调用音频管理器的设置BGM音量方法
        // AudioManager.Instance.SetBGMVolume(value / 10f);
    }

    private void OnVoiceVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("VoiceVolume", value);
        // 这里可以调用音频管理器的设置语音音量方法
        // AudioManager.Instance.SetVoiceVolume(value / 10f);
    }

    private void OnWindowModeChanged(int index)
    {
        PlayerPrefs.SetInt("WindowMode", index);
        bool isFullscreen = index == 0;
        Screen.fullScreen = isFullscreen;
    }

    private void OnResolutionChanged(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
    }

    private void OpenKeyConfig()
    {
        gameObject.SetActive(false); // 关闭设置面板
        InGameMenuManager.Instance.keyConfigPanel.SetActive(true); // 打开按键设置面板
    }

    private void OnBackButtonClicked()
    {
        InGameMenuManager.Instance.ReturnToMainMenu();
    }
}