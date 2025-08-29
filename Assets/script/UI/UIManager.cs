using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    // 单例模式
    public static UIManager Instance { get; private set; }

    // 定义界面类型枚举
    public enum UIPanelType
    {
        None,
        StartMenu,
        LoadMenu,
        SettingsMenu,
        KeySettings,
        GameInterface,
        InGameMenu,
        ItemMenu,
        TaskMenu,
        CollaborationMenu,
        LogMenu,
        MessagePopup,
        Dialogue,
        InteractionPrompt,
        PanelInterface
    }

    // 界面状态栈（后退栈）
    private Stack<UIPanelType> uiStack = new Stack<UIPanelType>();
    
    // 当前界面类型
    public UIPanelType CurrentPanelType { get; private set; } = UIPanelType.None;

    // 界面组引用
    [Header("UI Groups")]
    public GameObject startGroup;
    public GameObject loadGroup;
    public GameObject settingsGroup;
    public GameObject keySettingsGroup;
    public GameObject inGameMenuGroup;
    public GameObject itemMenuGroup;
    public GameObject taskMenuGroup;
    public GameObject collaborationMenuGroup;
    public GameObject logMenuGroup;
    public GameObject messageGroup;
    public GameObject dialogueGroup;
    public GameObject interactionPromptGroup;
    public GameObject panelInterfaceGroup;

    // 事件系统
    public UnityEvent<UIPanelType> onPanelChanged = new UnityEvent<UIPanelType>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始时显示开始界面
        SetRootPanel(UIPanelType.StartMenu);
    }

    private void Update()
    {
        // 监听ESC键和鼠标右键，实现全局返回逻辑
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            HandleBackAction();
        }
    }

    /// <summary>
    /// 切换到指定界面
    /// </summary>
    public void SwitchToPanel(UIPanelType panelType)
    {
        // 如果当前已有界面，先压入栈
        if (CurrentPanelType != UIPanelType.None && CurrentPanelType != panelType)
        {
            uiStack.Push(CurrentPanelType);
        }

        CurrentPanelType = panelType;
        Debug.Log("当前界面: " + panelType);

        // 触发界面切换事件
        OnPanelChange(panelType);
    }

    /// <summary>
    /// 返回上一个界面
    /// </summary>
    public bool ReturnToPreviousPanel()
    {
        if (uiStack.Count > 0)
        {
            CurrentPanelType = uiStack.Pop();
            Debug.Log("返回界面: " + CurrentPanelType);

            // 触发界面切换事件
            OnPanelChange(CurrentPanelType);
            return true;
        }
        else
        {
            // 如果栈为空，表示已经到了根界面
            if (CurrentPanelType != UIPanelType.StartMenu && CurrentPanelType != UIPanelType.GameInterface)
            {
                // 默认返回开始界面
                CurrentPanelType = UIPanelType.StartMenu;
                OnPanelChange(CurrentPanelType);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 清空栈，设置为指定根界面
    /// </summary>
    public void SetRootPanel(UIPanelType panelType)
    {
        uiStack.Clear();
        CurrentPanelType = panelType;
        OnPanelChange(panelType);
    }

    /// <summary>
    /// 处理返回操作
    /// </summary>
    public bool HandleBackAction()
    {
        // 检查当前界面是否有特殊的返回逻辑
        switch (CurrentPanelType)
        {
            case UIPanelType.MessagePopup:
                HideMessagePopup();
                return true;
            case UIPanelType.Dialogue:
                // 对话界面的返回逻辑由DialogueManager处理
                return false;
            default:
                return ReturnToPreviousPanel();
        }
    }

    /// <summary>
    /// 界面切换时的处理逻辑
    /// </summary>
    public void OnPanelChange(UIPanelType panelType)
    {
        // 关闭所有界面组
        CloseAllPanels();

        // 打开指定的界面组
        switch (panelType)
        {
            case UIPanelType.StartMenu:
                if (startGroup != null) startGroup.SetActive(true);
                break;
            case UIPanelType.LoadMenu:
                if (loadGroup != null) loadGroup.SetActive(true);
                break;
            case UIPanelType.SettingsMenu:
                if (settingsGroup != null) settingsGroup.SetActive(true);
                break;
            case UIPanelType.KeySettings:
                if (keySettingsGroup != null) keySettingsGroup.SetActive(true);
                break;
            case UIPanelType.InGameMenu:
                if (inGameMenuGroup != null) inGameMenuGroup.SetActive(true);
                break;
            case UIPanelType.ItemMenu:
                if (itemMenuGroup != null) itemMenuGroup.SetActive(true);
                break;
            case UIPanelType.TaskMenu:
                if (taskMenuGroup != null) taskMenuGroup.SetActive(true);
                break;
            case UIPanelType.CollaborationMenu:
                if (collaborationMenuGroup != null) collaborationMenuGroup.SetActive(true);
                break;
            case UIPanelType.LogMenu:
                if (logMenuGroup != null) logMenuGroup.SetActive(true);
                break;
            case UIPanelType.MessagePopup:
                if (messageGroup != null) messageGroup.SetActive(true);
                break;
            case UIPanelType.Dialogue:
                if (dialogueGroup != null) dialogueGroup.SetActive(true);
                break;
            case UIPanelType.PanelInterface:
                if (panelInterfaceGroup != null) panelInterfaceGroup.SetActive(true);
                break;
            // GameInterface和InteractionPrompt通常在游戏场景中处理，不需要在这里统一管理
        }

        // 触发事件
        onPanelChanged.Invoke(panelType);
    }

    /// <summary>
    /// 关闭所有界面组
    /// </summary>
    private void CloseAllPanels()
    {
        if (startGroup != null) startGroup.SetActive(false);
        if (loadGroup != null) loadGroup.SetActive(false);
        if (settingsGroup != null) settingsGroup.SetActive(false);
        if (keySettingsGroup != null) keySettingsGroup.SetActive(false);
        if (inGameMenuGroup != null) inGameMenuGroup.SetActive(false);
        if (itemMenuGroup != null) itemMenuGroup.SetActive(false);
        if (taskMenuGroup != null) taskMenuGroup.SetActive(false);
        if (collaborationMenuGroup != null) collaborationMenuGroup.SetActive(false);
        if (logMenuGroup != null) logMenuGroup.SetActive(false);
        if (messageGroup != null) messageGroup.SetActive(false);
        if (dialogueGroup != null) dialogueGroup.SetActive(false);
        if (panelInterfaceGroup != null) panelInterfaceGroup.SetActive(false);
    }

    /// <summary>
    /// 显示消息弹窗
    /// </summary>
    public void ShowMessagePopup()
    {
        if (messageGroup != null)
        {
            messageGroup.SetActive(true);
            // 消息弹窗不改变当前界面状态，但会压入栈中
            uiStack.Push(CurrentPanelType);
            CurrentPanelType = UIPanelType.MessagePopup;
        }
    }

    /// <summary>
    /// 隐藏消息弹窗
    /// </summary>
    public void HideMessagePopup()
    {
        if (messageGroup != null)
        {
            messageGroup.SetActive(false);
            // 返回之前的界面
            ReturnToPreviousPanel();
        }
    }

    /// <summary>
    /// 获取当前栈的大小
    /// </summary>
    public int GetStackSize()
    {
        return uiStack.Count;
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartNewGame()
    {
        // 初始化新游戏数据
        // 加载游戏场景
        // 切换到游戏界面
        SetRootPanel(UIPanelType.GameInterface);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑模式下退出
#endif
    }
}