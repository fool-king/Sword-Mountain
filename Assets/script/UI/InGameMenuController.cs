using UnityEngine;
using TMPro;

public class InGameMenuController : MonoBehaviour, IUIPanel
{
    public UIManager.UIPanelType PanelType => UIManager.UIPanelType.InGameMenu;
    
    [Header("游戏状态显示")]
    [SerializeField] private TextMeshProUGUI gameDayText;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    
    [Header("按键提示")]
    [SerializeField] private TextMeshProUGUI keyHintText;
    [SerializeField] private float hintShowDelay = 3f; // 无输入后显示提示的延迟时间（秒）
    
    private int currentDay = 1;
    private int currentActionPoints = 0;
    private int maxActionPoints = 10; // 每日初始行动点数量
    
    private float lastInputTime = 0f;
    private bool isHintVisible = false;
    
    private void Awake()
    {
        // 初始化游戏状态
        ResetActionPointsForNewDay();
        UpdateGameDayDisplay();
        UpdateActionPointsDisplay();
        
        // 初始化按键提示
        keyHintText.gameObject.SetActive(false);
        UpdateKeyHintDisplay();
        
        // 注册事件监听（假设存在GameStateManager或类似系统）
        // GameStateManager.Instance.OnDayChanged += OnDayChanged;
        // GameStateManager.Instance.OnActionPointsChanged += OnActionPointsChanged;
        
        // 监听按键配置变更
        KeyRebindingSystem.Instance.OnBindingsChanged += UpdateKeyHintDisplay;
    }
    
    private void OnDestroy()
    {
        // 取消事件监听
        // if (GameStateManager.Instance != null)
        // {
        //     GameStateManager.Instance.OnDayChanged -= OnDayChanged;
        //     GameStateManager.Instance.OnActionPointsChanged -= OnActionPointsChanged;
        // }
        
        if (KeyRebindingSystem.Instance != null)
        {
            KeyRebindingSystem.Instance.OnBindingsChanged -= UpdateKeyHintDisplay;
        }
    }
    
    private void Update()
    {
        CheckForInput();
        UpdateKeyHintVisibility();
    }
    
    public void OnShow()
    {
        // 界面显示时更新所有状态
        UpdateGameDayDisplay();
        UpdateActionPointsDisplay();
        UpdateKeyHintDisplay();
    }
    
    public void OnHide()
    {
        // 界面隐藏时的处理
    }
    
    public bool HandleBackAction()
    {
        // 处理返回操作
        return false; // 不阻止返回事件冒泡
    }
    
    #region 游戏状态管理
    
    // 重置新一天的行动点
    public void ResetActionPointsForNewDay()
    {
        currentActionPoints = maxActionPoints;
        UpdateActionPointsDisplay();
    }
    
    // 消耗行动点
    public bool ConsumeActionPoints(int amount)
    {
        if (currentActionPoints >= amount)
        {
            currentActionPoints -= amount;
            UpdateActionPointsDisplay();
            
            // 检查行动点是否用完
            if (currentActionPoints <= 0)
            {
                EndDay();
            }
            
            return true;
        }
        
        // 行动点不足，显示提示
        UIManager.Instance.ShowMessagePopup("行动点不足！");
        return false;
    }
    
    // 增加行动点（例如通过某些道具或技能）
    public void AddActionPoints(int amount)
    {
        currentActionPoints = Mathf.Min(currentActionPoints + amount, maxActionPoints);
        UpdateActionPointsDisplay();
    }
    
    // 结束当前天，进入下一天
    private void EndDay()
    {
        currentDay++;
        UpdateGameDayDisplay();
        ResetActionPointsForNewDay();
        
        // 显示天数变更提示
        UIManager.Instance.ShowMessagePopup("第" + currentDay + "天开始！");
        
        // 可以在这里触发新一天的事件
        // GameStateManager.Instance.TriggerNewDayEvent();
    }
    
    // 更新游戏天数显示
    private void UpdateGameDayDisplay()
    {
        if (gameDayText != null)
        {
            gameDayText.text = "第" + currentDay + "天";
        }
    }
    
    // 更新行动点显示
    private void UpdateActionPointsDisplay()
    {
        if (actionPointsText != null)
        {
            actionPointsText.text = "行动点：" + currentActionPoints + "/" + maxActionPoints;
        }
    }
    
    #endregion
    
    #region 按键提示管理
    
    // 检查用户输入
    private void CheckForInput()
    {
        // 检查键盘输入
        if (Input.anyKeyDown)
        {
            lastInputTime = Time.time;
            
            // 立即隐藏按键提示
            if (isHintVisible)
            {
                HideKeyHint();
            }
        }
    }
    
    // 更新按键提示可见性
    private void UpdateKeyHintVisibility()
    {
        if (!isHintVisible && Time.time - lastInputTime >= hintShowDelay)
        {
            ShowKeyHint();
        }
    }
    
    // 显示按键提示
    private void ShowKeyHint()
    {
        isHintVisible = true;
        keyHintText.gameObject.SetActive(true);
    }
    
    // 隐藏按键提示
    private void HideKeyHint()
    {
        isHintVisible = false;
        keyHintText.gameObject.SetActive(false);
    }
    
    // 更新按键提示内容
    private void UpdateKeyHintDisplay()
    {
        if (keyHintText == null)
            return;
        
        System.Text.StringBuilder hintBuilder = new System.Text.StringBuilder();
        
        // 添加游戏内主要快捷键提示
        AddKeyHint(hintBuilder, "打开菜单", "OpenMenu");
        AddKeyHint(hintBuilder, "打开物品栏", "OpenInventory");
        AddKeyHint(hintBuilder, "打开任务列表", "OpenTaskList");
        AddKeyHint(hintBuilder, "打开日志", "OpenLog");
        AddKeyHint(hintBuilder, "互动", "Interact");
        AddKeyHint(hintBuilder, "移动 - 上", "MoveUp");
        AddKeyHint(hintBuilder, "移动 - 下", "MoveDown");
        AddKeyHint(hintBuilder, "移动 - 左", "MoveLeft");
        AddKeyHint(hintBuilder, "移动 - 右", "MoveRight");
        
        keyHintText.text = hintBuilder.ToString();
    }
    
    // 向提示文本添加单个按键提示
    private void AddKeyHint(System.Text.StringBuilder builder, string actionName, string actionId)
    {
        KeyBinding binding = KeyRebindingSystem.Instance.GetBinding(actionId);
        if (binding != null)
        {
            if (builder.Length > 0)
            {
                builder.AppendLine();
            }
            builder.Append(actionName + ": " + binding.key.ToString());
        }
    }
    
    #endregion
    
    #region 外部接口
    
    // 获取当前游戏天数
    public int GetCurrentDay()
    {
        return currentDay;
    }
    
    // 设置当前游戏天数
    public void SetCurrentDay(int day)
    {
        currentDay = day;
        UpdateGameDayDisplay();
    }
    
    // 获取当前行动点
    public int GetCurrentActionPoints()
    {
        return currentActionPoints;
    }
    
    // 设置最大行动点
    public void SetMaxActionPoints(int maxPoints)
    {
        maxActionPoints = maxPoints;
        // 如果当前行动点超过新的最大值，进行调整
        currentActionPoints = Mathf.Min(currentActionPoints, maxActionPoints);
        UpdateActionPointsDisplay();
    }
    
    #endregion
}