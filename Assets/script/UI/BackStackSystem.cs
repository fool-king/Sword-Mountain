using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 后退栈管理系统，负责处理界面的后退逻辑
/// 支持ESC键和鼠标右键返回操作
/// </summary>
public class BackStackSystem : MonoBehaviour
{
    // 单例模式
    public static BackStackSystem Instance { get; private set; }

    /// <summary>
    /// 界面返回栈，用于存储界面历史记录
    /// </summary>
    private Stack<UIManager.UIPanelType> uiBackStack = new Stack<UIManager.UIPanelType>();

    /// <summary>
    /// 当前根界面类型
    /// </summary>
    private UIManager.UIPanelType rootPanelType = UIManager.UIPanelType.StartMenu;

    /// <summary>
    /// 自定义返回处理委托
    /// </summary>
    public delegate bool CustomBackHandler();

    /// <summary>
    /// 当前激活的自定义返回处理器
    /// </summary>
    private List<CustomBackHandler> customBackHandlers = new List<CustomBackHandler>();

    /// <summary>
    /// 当返回操作被处理时触发的事件
    /// </summary>
    public event Action OnBackActionHandled;

    /// <summary>
    /// 当返回栈为空时触发的事件
    /// </summary>
    public event Action OnBackStackEmpty;

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

    private void Update()
    {
        // 监听ESC键和鼠标右键，实现全局返回逻辑
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            HandleBackAction();
        }
    }

    /// <summary>
    /// 处理返回操作
    /// </summary>
    /// <returns>是否成功处理了返回操作</returns>
    public bool HandleBackAction()
    {
        // 首先尝试调用自定义返回处理器
        if (TryCustomBackHandlers())
        {
            OnBackActionHandled?.Invoke();
            return true;
        }

        // 如果自定义处理器没有处理，则执行默认的栈返回逻辑
        return ReturnToPreviousPanel();
    }

    /// <summary>
    /// 尝试通过自定义返回处理器处理返回操作
    /// </summary>
    /// <returns>是否成功处理</returns>
    private bool TryCustomBackHandlers()
    {
        // 从后向前遍历（后进先出原则）
        for (int i = customBackHandlers.Count - 1; i >= 0; i--)
        {
            if (customBackHandlers[i]?.Invoke() ?? false)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 返回上一个界面
    /// </summary>
    /// <returns>是否成功返回</returns>
    public bool ReturnToPreviousPanel()
    {
        if (uiBackStack.Count > 0)
        {
            UIManager.UIPanelType previousPanel = uiBackStack.Pop();
            Debug.Log("返回上一个界面: " + previousPanel);
            UIManager.Instance.SwitchToPanel(previousPanel);
            OnBackActionHandled?.Invoke();
            return true;
        }
        else
        {
            // 返回栈为空时的处理
            HandleEmptyStack();
            return false;
        }
    }

    /// <summary>
    /// 处理返回栈为空的情况
    /// </summary>
    private void HandleEmptyStack()
    {
        // 获取当前UIManager的游戏状态
        UIManager.GameState currentGameState = UIManager.Instance.CurrentGameState;

        switch (currentGameState)
        {
            case UIManager.GameState.MainMenu:
                // 在主菜单状态下，如果已经在根界面，不执行任何操作
                Debug.Log("返回栈为空，已在根界面");
                break;
            case UIManager.GameState.InGame:
            case UIManager.GameState.InDialogue:
            case UIManager.GameState.InMenu:
                // 在游戏中状态下，如果返回栈为空，切换到游戏界面
                UIManager.Instance.SwitchToPanel(UIManager.UIPanelType.GameInterface);
                break;
            default:
                Debug.LogWarning("未处理的游戏状态: " + currentGameState);
                break;
        }

        OnBackStackEmpty?.Invoke();
    }

    /// <summary>
    /// 将当前界面压入返回栈
    /// </summary>
    /// <param name="panelType">要压入栈的界面类型</param>
    public void PushPanel(UIManager.UIPanelType panelType)
    {
        // 避免重复压入同一个界面
        if (uiBackStack.Count == 0 || uiBackStack.Peek() != panelType)
        {
            uiBackStack.Push(panelType);
            Debug.Log("压入界面到返回栈: " + panelType + ", 当前栈大小: " + uiBackStack.Count);
        }
    }

    /// <summary>
    /// 清空返回栈
    /// </summary>
    public void ClearStack()
    {
        uiBackStack.Clear();
        Debug.Log("返回栈已清空");
    }

    /// <summary>
    /// 设置根界面类型
    /// </summary>
    /// <param name="panelType">根界面类型</param>
    public void SetRootPanel(UIManager.UIPanelType panelType)
    {
        rootPanelType = panelType;
        ClearStack();
        Debug.Log("设置根界面: " + panelType);
    }

    /// <summary>
    /// 获取当前返回栈的大小
    /// </summary>
    /// <returns>栈的大小</returns>
    public int GetStackSize()
    { return uiBackStack.Count; }

    /// <summary>
    /// 获取返回栈顶元素
    /// </summary>
    /// <returns>栈顶界面类型，如果栈为空则返回None</returns>
    public UIManager.UIPanelType PeekStack()
    { return uiBackStack.Count > 0 ? uiBackStack.Peek() : UIManager.UIPanelType.None; }

    /// <summary>
    /// 注册自定义返回处理器
    /// </summary>
    /// <param name="handler">自定义返回处理器委托</param>
    public void RegisterCustomBackHandler(CustomBackHandler handler)
    {
        if (handler != null && !customBackHandlers.Contains(handler))
        {
            customBackHandlers.Add(handler);
            Debug.Log("注册自定义返回处理器，当前处理器数量: " + customBackHandlers.Count);
        }
    }

    /// <summary>
    /// 取消注册自定义返回处理器
    /// </summary>
    /// <param name="handler">要取消注册的处理器委托</param>
    public void UnregisterCustomBackHandler(CustomBackHandler handler)
    {
        if (handler != null && customBackHandlers.Contains(handler))
        {
            customBackHandlers.Remove(handler);
            Debug.Log("取消注册自定义返回处理器，当前处理器数量: " + customBackHandlers.Count);
        }
    }

    /// <summary>
    /// 清空所有自定义返回处理器
    /// </summary>
    public void ClearCustomBackHandlers()
    {
        customBackHandlers.Clear();
        Debug.Log("清空所有自定义返回处理器");
    }

    /// <summary>
    /// 检测某个界面是否在返回栈中
    /// </summary>
    /// <param name="panelType">要检测的界面类型</param>
    /// <returns>是否在栈中</returns>
    public bool IsPanelInStack(UIManager.UIPanelType panelType)
    {
        return uiBackStack.Contains(panelType);
    }

    /// <summary>
    /// 从返回栈中移除特定界面
    /// </summary>
    /// <param name="panelType">要移除的界面类型</param>
    /// <returns>是否成功移除</returns>
    public bool RemovePanelFromStack(UIManager.UIPanelType panelType)
    {
        // 如果要直接操作栈，需要临时转换为数组
        if (uiBackStack.Contains(panelType))
        {
            UIManager.UIPanelType[] tempArray = uiBackStack.ToArray();
            uiBackStack.Clear();
            
            foreach (UIManager.UIPanelType type in tempArray)
            {
                if (type != panelType)
                {
                    uiBackStack.Push(type);
                }
            }
            
            // 重新反转栈，因为ToArray后顺序会颠倒
            tempArray = uiBackStack.ToArray();
            uiBackStack.Clear();
            for (int i = tempArray.Length - 1; i >= 0; i--)
            {
                uiBackStack.Push(tempArray[i]);
            }
            
            return true;
        }
        return false;
    }
}