using UnityEngine;

public class StartMenuController : MonoBehaviour, IUIPanel
{
    public UIManager.UIPanelType PanelType => UIManager.UIPanelType.StartMenu;
    
    // 在Inspector中拖拽赋值
    public UnityEngine.UI.Button newButton;
    public UnityEngine.UI.Button loadButton;
    public UnityEngine.UI.Button configButton;
    public UnityEngine.UI.Button endButton;

    void Start()
    {
        // 为按钮添加监听事件
        newButton.onClick.AddListener(OnNewGameClicked);
        loadButton.onClick.AddListener(OnLoadGameClicked);
        configButton.onClick.AddListener(OnConfigClicked);
        endButton.onClick.AddListener(OnEndGameClicked);
    }

    public void OnShow() { }
    
    public void OnHide() { }
    
    public bool HandleBackAction() { return false; }

    private void OnNewGameClicked()
    {
        // 切换到Load界面，并告知它是"New Game"模式
        UIManager.Instance.SwitchMenuGroup(UIManager.Instance.loadGroup);
        FindObjectOfType<LoadMenuController>().SetMode(isLoadMode: false);
    }

    private void OnLoadGameClicked()
    {
        // 切换到Load界面，并告知它是"Load Game"模式
        UIManager.Instance.SwitchMenuGroup(UIManager.Instance.loadGroup);
        FindObjectOfType<LoadMenuController>().SetMode(isLoadMode: true);
    }

    private void OnConfigClicked()
    {
        UIManager.Instance.SwitchMenuGroup(UIManager.Instance.configGroup);
    }

    private void OnEndGameClicked()
    {
        Debug.Log("退出游戏");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑模式下退出
#endif
    }
}