public class SettingsMenuController : MonoBehaviour, IUIPanel
{
    public UIManager.UIPanelType PanelType => UIManager.UIPanelType.SettingsMenu;
    
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    
    public void OnShow()
    {
        // 从SettingsManager加载当前设置
        GameSettings settings = SettingsManager.Instance.GetCurrentSettings();
        bgmSlider.value = settings.bgmVolume;
        sfxSlider.value = settings.sfxVolume;
        fullscreenToggle.isOn = settings.isFullscreen;
    }
    
    public void OnHide()
    {
        // 保存设置
        SettingsManager.Instance.SaveSettings();
    }
    
    public bool HandleBackAction()
    {
        UIManager.Instance.ReturnToPreviousPanel();
        return true;
    }
    
    // 本界面的独特功能：实时调整设置
    public void OnBGMVolumeChanged(float value)
    {
        SettingsManager.Instance.SetBGMVolume(value);
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        SettingsManager.Instance.SetSFXVolume(value);
    }
    
    public void OnFullscreenToggleChanged(bool value)
    {
        SettingsManager.Instance.SetWindowMode(value);
    }
    
    // 跳转到按键设置界面（已由UIManager实现）
    public void OnKeySettingsButtonClicked()
    {
        UIManager.Instance.SwitchToPanel(UIManager.UIPanelType.KeySettings);
    }
}