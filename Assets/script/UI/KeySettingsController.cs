using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeySettingsController : MonoBehaviour, IUIPanel
{
    public UIManager.UIPanelType PanelType => UIManager.UIPanelType.KeySettings;
    
    [System.Serializable]
    public struct KeyBindingUIItem
    {
        public TextMeshProUGUI actionNameText;
        public TextMeshProUGUI keyNameText;
        public Button rebindButton;
        public string actionId;
    }
    
    [SerializeField] private KeyBindingUIItem[] keyBindings;
    [SerializeField] private Button resetToDefaultsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject rebindPromptPanel;
    [SerializeField] private TextMeshProUGUI rebindPromptText;
    
    private bool isRebinding = false;
    private string currentActionId = null;
    
    private void Awake()
    {
        // 设置按钮事件
        resetToDefaultsButton.onClick.AddListener(OnResetToDefaults);
        backButton.onClick.AddListener(OnBackButtonClicked);
        
        // 设置所有按键绑定的UI项
        for (int i = 0; i < keyBindings.Length; i++)
        {
            int index = i; // 闭包变量捕获
            keyBindings[i].rebindButton.onClick.AddListener(() => OnRebindButtonClicked(keyBindings[index].actionId));
        }
        
        // 初始化时隐藏重绑定提示面板
        rebindPromptPanel.SetActive(false);
    }
    
    public void OnShow()
    {
        UpdateKeyBindingDisplay();
    }
    
    public void OnHide()
    {
        // 取消任何正在进行的重绑定操作
        if (isRebinding)
        {
            CancelRebinding();
        }
    }
    
    public bool HandleBackAction()
    {
        if (isRebinding)
        {
            CancelRebinding();
            return true;
        }
        
        OnBackButtonClicked();
        return true;
    }
    
    private void Update()
    {
        // 处理重绑定过程中的按键输入
        if (isRebinding)
        {
            ProcessKeyRebindingInput();
        }
    }
    
    private void UpdateKeyBindingDisplay()
    {
        // 更新所有按键绑定的显示
        for (int i = 0; i < keyBindings.Length; i++)
        {
            KeyBinding binding = KeyRebindingSystem.Instance.GetBinding(keyBindings[i].actionId);
            if (binding != null)
            {
                keyBindings[i].keyNameText.text = binding.key.ToString();
            }
        }
    }
    
    private void OnRebindButtonClicked(string actionId)
    {
        if (isRebinding)
        {
            // 如果已经在重绑定过程中，先取消
            CancelRebinding();
        }
        
        // 开始新的重绑定过程
        isRebinding = true;
        currentActionId = actionId;
        
        // 显示重绑定提示面板
        rebindPromptPanel.SetActive(true);
        
        // 找到对应的操作名称
        string actionName = "";
        for (int i = 0; i < keyBindings.Length; i++)
        {
            if (keyBindings[i].actionId == actionId)
            {
                actionName = keyBindings[i].actionNameText.text;
                break;
            }
        }
        
        rebindPromptText.text = string.Format("按下新按键以重新绑定 '{0}'\nESC键取消", actionName);
        
        // 禁用其他UI交互
        ToggleUIInteraction(false);
    }
    
    private void ProcessKeyRebindingInput()
    {
        if (!isRebinding || currentActionId == null)
            return;
        
        // 检查按键输入
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                // 检查是否是取消按键
                if (key == KeyCode.Escape)
                {
                    CancelRebinding();
                    return;
                }
                
                // 检查是否是系统保留按键
                if (IsSystemReservedKey(key))
                {
                    rebindPromptText.text = string.Format("该按键是系统保留按键，不能绑定\n按下新按键以重新绑定\nESC键取消");
                    return;
                }
                
                // 检查按键冲突
                string conflictingAction = KeyRebindingSystem.Instance.CheckBindingConflict(key);
                if (!string.IsNullOrEmpty(conflictingAction))
                {
                    // 显示冲突提示
                    UIManager.Instance.ShowMessagePopup(
                        string.Format("按键 {0} 已绑定到 '{1}'\n确定要替换吗？", 
                        key.ToString(), conflictingAction),
                        () => {
                            // 用户确认替换
                            CompleteRebinding(key);
                        },
                        null // 取消则继续等待输入
                    );
                    return;
                }
                
                // 完成重绑定
                CompleteRebinding(key);
                return;
            }
        }
    }
    
    private void CompleteRebinding(KeyCode newKey)
    {
        if (!isRebinding || currentActionId == null)
            return;
        
        // 执行重绑定
        KeyRebindingSystem.Instance.SetBinding(currentActionId, newKey);
        
        // 保存设置
        KeyRebindingSystem.Instance.SaveBindings();
        
        // 更新UI显示
        UpdateKeyBindingDisplay();
        
        // 结束重绑定过程
        EndRebinding();
    }
    
    private void CancelRebinding()
    {
        if (!isRebinding)
            return;
        
        // 显示取消提示
        UIManager.Instance.ShowMessagePopup("重绑定已取消");
        
        // 结束重绑定过程
        EndRebinding();
    }
    
    private void EndRebinding()
    {
        isRebinding = false;
        currentActionId = null;
        rebindPromptPanel.SetActive(false);
        
        // 启用UI交互
        ToggleUIInteraction(true);
    }
    
    private void ToggleUIInteraction(bool enabled)
    {
        // 启用或禁用UI交互
        resetToDefaultsButton.interactable = enabled;
        backButton.interactable = enabled;
        
        for (int i = 0; i < keyBindings.Length; i++)
        {
            keyBindings[i].rebindButton.interactable = enabled;
        }
    }
    
    private bool IsSystemReservedKey(KeyCode key)
    {
        // 定义系统保留按键
        return key == KeyCode.Escape || key == KeyCode.Mouse0 || key == KeyCode.Mouse1;
    }
    
    private void OnResetToDefaults()
    {
        // 显示确认提示
        UIManager.Instance.ShowMessagePopup(
            "确定要重置所有按键绑定为默认设置吗？",
            () => {
                // 用户确认重置
                KeyRebindingSystem.Instance.ResetToDefaults();
                KeyRebindingSystem.Instance.SaveBindings();
                UpdateKeyBindingDisplay();
                UIManager.Instance.ShowMessagePopup("按键设置已重置为默认值");
            }
        );
    }
    
    private void OnBackButtonClicked()
    {
        // 返回上一个界面
        UIManager.Instance.ReturnToPreviousPanel();
    }
}