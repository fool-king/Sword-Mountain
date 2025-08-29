public class LoadMenuController : MonoBehaviour, IUIPanel
{
    public UIManager.UIPanelType PanelType => UIManager.UIPanelType.LoadMenu;
    
    [SerializeField] private SaveSlotUI[] saveSlots;
    private LoadMenuContext currentContext;
    private GameSaveData currentSaveData; // 添加当前存档数据变量
    
    public void OnShow()
    {
        // 刷新存档数据显示
        RefreshSaveSlots();
    }
    
    public void OnHide()
    {
        // 清理操作
    }
    
    public bool HandleBackAction()
    {
        // 处理本界面的特殊返回逻辑
        UIManager.Instance.ReturnToPreviousPanel();
        return true;
    }
    
    public void SetContext(LoadMenuContext context)
    {
        currentContext = context;
        currentSaveData = context.SaveData; // 从上下文中获取存档数据
        UpdateUIForContext();
    }
    
    // 添加设置模式的方法
    public void SetMode(bool isLoadMode)
    {
        if (currentContext == null)
            currentContext = new LoadMenuContext();
            
        currentContext.IsLoadMode = isLoadMode;
        UpdateUIForContext();
    }
    
    private void UpdateUIForContext()
    {
        // 根据上下文更新UI显示
    }
    
    private void RefreshSaveSlots()
    {
        // 调用SaveLoadSystem获取数据并更新UI
        for (int i = 0; i < saveSlots.Length; i++)
        {
            SaveSlotInfo info = SaveLoadSystem.Instance.GetSaveSlotInfo(i);
            saveSlots[i].UpdateDisplay(info);
        }
    }
    
    // 存档槽点击事件（本界面的独特功能）
    public void OnSaveSlotClicked(int slotIndex)
    {
        if (currentContext.IsLoadMode)
        {
            // 载入存档逻辑
            GameSaveData data = SaveLoadSystem.Instance.LoadSaveData(slotIndex);
            if (data != null)
            {
                // 处理载入逻辑...
                UIManager.Instance.ShowMessagePopup("加载成功！", () => {
                    // 这里可以添加进入游戏的逻辑
                });
            }
            else
            {
                UIManager.Instance.ShowMessagePopup("存档不存在！");
            }
        }
        else
        {
            // 保存存档逻辑
            UIManager.Instance.ShowMessagePopup("确认覆盖存档？", 
                () => {
                    if (currentSaveData == null)
                        currentSaveData = new GameSaveData(); // 如果没有数据，创建新数据
                    
                    SaveLoadSystem.Instance.SaveGame(slotIndex, currentSaveData);
                    RefreshSaveSlots();
                });
        }
    }
}