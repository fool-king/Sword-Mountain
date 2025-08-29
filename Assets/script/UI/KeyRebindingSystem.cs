using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

/// <summary>
/// 输入类型枚举
/// </summary>
public enum InputType
{
    Keyboard,
    Mouse,
    Controller,
    None
}

/// <summary>
/// 按键绑定数据结构
/// </summary>
[System.Serializable]
public class KeyBinding
{
    public string actionName;            // 动作名称
    public string displayName;           // 显示名称
    public KeyCode defaultKey;           // 默认按键
    public KeyCode currentKey;           // 当前按键
    public InputType inputType;          // 输入类型
    public bool isRebindable = true;     // 是否可重绑定
    public string category = "General";  // 分类

    public KeyBinding(string actionName, string displayName, KeyCode defaultKey)
    {
        this.actionName = actionName;
        this.displayName = displayName;
        this.defaultKey = defaultKey;
        this.currentKey = defaultKey;
        this.inputType = GetInputTypeFromKeyCode(defaultKey);
    }

    /// <summary>
    /// 从KeyCode确定输入类型
    /// </summary>
    /// <param name="keyCode">按键代码</param>
    /// <returns>输入类型</returns>
    private InputType GetInputTypeFromKeyCode(KeyCode keyCode)
    {
        if (keyCode >= KeyCode.Mouse0 && keyCode <= KeyCode.Mouse6)
            return InputType.Mouse;
        else if (keyCode >= KeyCode.Joystick1Button0 && keyCode <= KeyCode.Joystick8Button19)
            return InputType.Controller;
        else if (keyCode != KeyCode.None)
            return InputType.Keyboard;
        else
            return InputType.None;
    }

    /// <summary>
    /// 重置为默认值
    /// </summary>
    public void ResetToDefault()
    {
        currentKey = defaultKey;
        inputType = GetInputTypeFromKeyCode(defaultKey);
    }
}

/// <summary>
/// 按键重绑定系统
/// </summary>
public class KeyRebindingSystem : MonoBehaviour
{
    public static KeyRebindingSystem Instance { get; private set; }

    private List<KeyBinding> keyBindings = new List<KeyBinding>();
    private bool isWaitingForKey = false;
    private KeyBinding currentBindingToRebind = null;
    private System.Action<KeyBinding> onRebindComplete = null;
    private System.Action onRebindCancel = null;

    // 不允许重绑定的特殊按键
    private readonly List<KeyCode> restrictedKeys = new List<KeyCode>
    {
        KeyCode.Escape,
        KeyCode.LeftWindows,
        KeyCode.RightWindows,
        KeyCode.LeftCommand,
        KeyCode.RightCommand
    };

    public event System.Action OnKeyBindingsChanged; // 按键绑定变更事件

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultBindings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化默认按键绑定
    /// </summary>
    private void InitializeDefaultBindings()
    {
        keyBindings = new List<KeyBinding>
        {
            // 移动控制
            new KeyBinding("MoveForward", "向前移动", KeyCode.W),
            new KeyBinding("MoveBackward", "向后移动", KeyCode.S),
            new KeyBinding("MoveLeft", "向左移动", KeyCode.A),
            new KeyBinding("MoveRight", "向右移动", KeyCode.D),
            new KeyBinding("Jump", "跳跃", KeyCode.Space),
            new KeyBinding("Crouch", "蹲下", KeyCode.LeftControl),
            
            // 交互控制
            new KeyBinding("Interact", "互动", KeyCode.E),
            new KeyBinding("Attack", "攻击", KeyCode.Mouse0),
            new KeyBinding("Defend", "防御", KeyCode.Mouse1),
            new KeyBinding("UseSkill1", "技能1", KeyCode.Alpha1),
            new KeyBinding("UseSkill2", "技能2", KeyCode.Alpha2),
            new KeyBinding("UseSkill3", "技能3", KeyCode.Alpha3),
            new KeyBinding("UseSkill4", "技能4", KeyCode.Alpha4),
            
            // 界面控制
            new KeyBinding("Inventory", "背包", KeyCode.I),
            new KeyBinding("CharacterMenu", "角色", KeyCode.C),
            new KeyBinding("SkillMenu", "技能", KeyCode.K),
            new KeyBinding("Map", "地图", KeyCode.M),
            new KeyBinding("QuestLog", "任务日志", KeyCode.L),
            new KeyBinding("Pause", "暂停菜单", KeyCode.Escape),
            
            // 其他控制
            new KeyBinding("ToggleRun", "切换奔跑", KeyCode.LeftShift),
            new KeyBinding("ZoomIn", "镜头拉近", KeyCode.KeypadPlus),
            new KeyBinding("ZoomOut", "镜头拉远", KeyCode.KeypadMinus)
        };
    }

    /// <summary>
    /// 获取所有按键绑定
    /// </summary>
    /// <returns>按键绑定列表</returns>
    public List<KeyBinding> GetAllKeyBindings()
    {
        return new List<KeyBinding>(keyBindings);
    }

    /// <summary>
    /// 获取指定分类的按键绑定
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>按键绑定列表</returns>
    public List<KeyBinding> GetKeyBindingsByCategory(string category)
    {
        return keyBindings.Where(binding => binding.category.Equals(category)).ToList();
    }

    /// <summary>
    /// 获取指定动作的按键绑定
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>按键绑定</returns>
    public KeyBinding GetKeyBinding(string actionName)
    {
        return keyBindings.Find(binding => binding.actionName.Equals(actionName));
    }

    /// <summary>
    /// 获取指定动作的当前按键
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>按键代码</returns>
    public KeyCode GetKeyForAction(string actionName)
    {
        KeyBinding binding = GetKeyBinding(actionName);
        return binding != null ? binding.currentKey : KeyCode.None;
    }

    /// <summary>
    /// 检查指定动作的按键是否被按下
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>是否被按下</returns>
    public bool IsActionPressed(string actionName)
    {
        KeyCode keyCode = GetKeyForAction(actionName);
        if (keyCode != KeyCode.None)
        {
            return Input.GetKey(keyCode);
        }
        return false;
    }

    /// <summary>
    /// 检查指定动作的按键是否被按下（单次触发）
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>是否被按下</returns>
    public bool IsActionPressedDown(string actionName)
    {
        KeyCode keyCode = GetKeyForAction(actionName);
        if (keyCode != KeyCode.None)
        {
            return Input.GetKeyDown(keyCode);
        }
        return false;
    }

    /// <summary>
    /// 开始重绑定按键
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <param name="onComplete">重绑定完成回调</param>
    /// <param name="onCancel">重绑定取消回调</param>
    /// <returns>是否成功开始重绑定</returns>
    public bool StartKeyRebinding(string actionName, 
                                 System.Action<KeyBinding> onComplete = null, 
                                 System.Action onCancel = null)
    {
        // 如果已经在等待按键输入，则取消之前的重绑定
        if (isWaitingForKey)
        {
            CancelKeyRebinding();
        }

        KeyBinding binding = GetKeyBinding(actionName);
        if (binding == null || !binding.isRebindable)
        {
            Debug.LogWarning("Cannot rebind key for action: " + actionName);
            return false;
        }

        isWaitingForKey = true;
        currentBindingToRebind = binding;
        onRebindComplete = onComplete;
        onRebindCancel = onCancel;

        Debug.Log("Waiting for key input for action: " + actionName);
        return true;
    }

    /// <summary>
    /// 取消按键重绑定
    /// </summary>
    public void CancelKeyRebinding()
    {
        if (isWaitingForKey)
        {
            isWaitingForKey = false;
            
            if (onRebindCancel != null)
            {
                onRebindCancel.Invoke();
            }
            
            currentBindingToRebind = null;
            onRebindComplete = null;
            onRebindCancel = null;
            
            Debug.Log("Key rebinding cancelled.");
        }
    }

    /// <summary>
    /// 设置按键绑定
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <param name="newKeyCode">新的按键代码</param>
    /// <returns>是否设置成功</returns>
    public bool SetKeyBinding(string actionName, KeyCode newKeyCode)
    {
        KeyBinding binding = GetKeyBinding(actionName);
        if (binding == null || !binding.isRebindable)
        {
            Debug.LogWarning("Cannot set key binding for action: " + actionName);
            return false;
        }

        // 检查是否是受限按键
        if (restrictedKeys.Contains(newKeyCode))
        {
            Debug.LogWarning("Cannot bind to restricted key: " + newKeyCode);
            return false;
        }

        // 检查按键冲突
        if (IsKeyAlreadyBound(newKeyCode, actionName))
        {
            Debug.LogWarning("Key already bound to another action: " + newKeyCode);
            return false;
        }

        // 更新按键绑定
        binding.currentKey = newKeyCode;
        binding.inputType = GetInputTypeFromKeyCode(newKeyCode);

        // 触发变更事件
        TriggerKeyBindingsChanged();

        // 保存设置
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
        }

        Debug.Log("Key binding set: " + actionName + " = " + newKeyCode);
        return true;
    }

    /// <summary>
    /// 重置所有按键绑定为默认值
    /// </summary>
    public void ResetAllKeyBindings()
    {
        foreach (KeyBinding binding in keyBindings)
        {
            if (binding.isRebindable)
            {
                binding.ResetToDefault();
            }
        }

        // 触发变更事件
        TriggerKeyBindingsChanged();

        // 保存设置
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
        }

        Debug.Log("All key bindings reset to default.");
    }

    /// <summary>
    /// 重置指定动作的按键绑定为默认值
    /// </summary>
    /// <param name="actionName">动作名称</param>
    /// <returns>是否重置成功</returns>
    public bool ResetKeyBinding(string actionName)
    {
        KeyBinding binding = GetKeyBinding(actionName);
        if (binding == null || !binding.isRebindable)
        {
            Debug.LogWarning("Cannot reset key binding for action: " + actionName);
            return false;
        }

        binding.ResetToDefault();

        // 触发变更事件
        TriggerKeyBindingsChanged();

        // 保存设置
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveSettings();
        }

        Debug.Log("Key binding reset to default: " + actionName);
        return true;
    }

    /// <summary>
    /// 检查按键是否已经被绑定到其他动作
    /// </summary>
    /// <param name="keyCode">按键代码</param>
    /// <param name="excludeActionName">排除的动作名称</param>
    /// <returns>是否已被绑定</returns>
    public bool IsKeyAlreadyBound(KeyCode keyCode, string excludeActionName = null)
    {
        return keyBindings.Any(binding => 
            binding.isRebindable && 
            binding.currentKey == keyCode && 
            binding.actionName != excludeActionName);
    }

    /// <summary>
    /// 从按键代码获取输入类型
    /// </summary>
    /// <param name="keyCode">按键代码</param>
    /// <returns>输入类型</returns>
    public InputType GetInputTypeFromKeyCode(KeyCode keyCode)
    {
        if (keyCode >= KeyCode.Mouse0 && keyCode <= KeyCode.Mouse6)
            return InputType.Mouse;
        else if (keyCode >= KeyCode.Joystick1Button0 && keyCode <= KeyCode.Joystick8Button19)
            return InputType.Controller;
        else if (keyCode != KeyCode.None)
            return InputType.Keyboard;
        else
            return InputType.None;
    }

    /// <summary>
    /// 从SettingsManager加载按键绑定
    /// </summary>
    public void LoadKeyBindingsFromSettings()
    {
        if (SettingsManager.Instance != null)
        {
            GameSettings settings = SettingsManager.Instance.GetCurrentSettings();
            if (settings != null && settings.keyBindings != null)
            {
                foreach (var binding in keyBindings)
                {
                    if (settings.keyBindings.ContainsKey(binding.actionName))
                    {
                        KeyCode savedKey = settings.keyBindings[binding.actionName];
                        // 检查按键是否有效且未被限制
                        if (savedKey != KeyCode.None && !restrictedKeys.Contains(savedKey))
                        {
                            binding.currentKey = savedKey;
                            binding.inputType = GetInputTypeFromKeyCode(savedKey);
                        }
                    }
                }
                
                Debug.Log("Key bindings loaded from settings.");
            }
        }
    }

    /// <summary>
    /// 保存按键绑定到SettingsManager
    /// </summary>
    public void SaveKeyBindingsToSettings()
    {
        if (SettingsManager.Instance != null)
        {
            GameSettings settings = SettingsManager.Instance.GetCurrentSettings();
            if (settings != null && settings.keyBindings != null)
            {
                foreach (var binding in keyBindings)
                {
                    if (binding.isRebindable)
                    {
                        settings.keyBindings[binding.actionName] = binding.currentKey;
                    }
                }
                
                SettingsManager.Instance.SaveSettings();
                Debug.Log("Key bindings saved to settings.");
            }
        }
    }

    /// <summary>
    /// 触发按键绑定变更事件
    /// </summary>
    private void TriggerKeyBindingsChanged()
    {
        if (OnKeyBindingsChanged != null)
        {
            OnKeyBindingsChanged.Invoke();
        }
    }

    private void Update()
    {
        // 处理按键重绑定
        if (isWaitingForKey && currentBindingToRebind != null)
        {
            HandleKeyRebindingInput();
        }
    }

    /// <summary>
    /// 处理按键重绑定输入
    /// </summary>
    private void HandleKeyRebindingInput()
    {
        // 检查是否按下ESC取消
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelKeyRebinding();
            return;
        }

        // 检查是否有按键输入
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    // 检查是否是有效的按键
                    if (keyCode != KeyCode.None && !restrictedKeys.Contains(keyCode))
                    {
                        // 尝试设置新的按键绑定
                        bool success = SetKeyBinding(currentBindingToRebind.actionName, keyCode);
                        
                        if (success && onRebindComplete != null)
                        {
                            onRebindComplete.Invoke(currentBindingToRebind);
                        }
                        
                        // 重置状态
                        isWaitingForKey = false;
                        currentBindingToRebind = null;
                        onRebindComplete = null;
                        onRebindCancel = null;
                        
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取按键绑定的调试信息
    /// </summary>
    /// <returns>调试信息字符串</returns>
    public string GetDebugInfo()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Key Rebinding System Debug Info:");
        sb.AppendLine("================================");
        
        // 按分类分组显示
        var groupedBindings = keyBindings.GroupBy(binding => binding.category);
        
        foreach (var group in groupedBindings)
        {
            sb.AppendLine($"\n{group.Key}:");
            foreach (var binding in group)
            {
                string rebindableStatus = binding.isRebindable ? "(可重绑定)" : "(不可重绑定)";
                sb.AppendLine($"  {binding.displayName}: {binding.currentKey} {rebindableStatus}