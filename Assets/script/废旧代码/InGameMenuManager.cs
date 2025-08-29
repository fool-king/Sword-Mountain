using UnityEngine;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour
{
    public static InGameMenuManager Instance;

    [Header("UI Groups")]
    public GameObject menuGroup; // 整个菜单组
    public GameObject settingsPanel; // 设置面板
    public GameObject keyConfigPanel; // 按键设置面板

    [Header("Menu Buttons")]
    public Button configButton;
    public Button itemButton;
    public Button taskButton;
    public Button collaborationButton;
    public Button logButton;
    public Button exitButton;
    public Button saveButton;
    public Button panelButton;

    private bool isMenuOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始状态：关闭菜单
        SetMenuActive(false);

        // 绑定菜单按钮事件
        configButton.onClick.AddListener(OpenSettings);
        // 其他按钮绑定类似，需要您补充完整
        // itemButton.onClick.AddListener(OpenInventory);
        // taskButton.onClick.AddListener(OpenQuest);
        // collaborationButton.onClick.AddListener(OpenCollaboration);
        // logButton.onClick.AddListener(OpenLog);
        // exitButton.onClick.AddListener(ExitToMainMenu);
        // saveButton.onClick.AddListener(SaveGame);
        // panelButton.onClick.AddListener(OpenPanel);
    }

    private void Update()
    {
        // 检测TAB键按下
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        SetMenuActive(isMenuOpen);

        // 菜单打开时暂停游戏，关闭时恢复
        Time.timeScale = isMenuOpen ? 0f : 1f;
    }

    private void SetMenuActive(bool active)
    {
        menuGroup.SetActive(active);
        
        // 确保打开菜单时关闭子面板
        if (!active)
        {
            settingsPanel.SetActive(false);
            keyConfigPanel.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        keyConfigPanel.SetActive(false);
    }

    public void CloseAllSubPanels()
    {
        settingsPanel.SetActive(false);
        keyConfigPanel.SetActive(false);
    }

    // 从子面板返回到主菜单
    public void ReturnToMainMenu()
    {
        CloseAllSubPanels();
    }
}