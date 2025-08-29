using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadMenuController : MonoBehaviour
{
    public Button[] slotButtons; // 分配4个存档槽按钮
    public TMP_Text[] slotInfoTexts; // 分配4个存档信息Text
    public Button yesButton;
    public Button backButton;

    private bool isLoadMode; // true=载入, false=新建
    private int selectedSlotIndex = -1; // 当前选中的存档槽索引（0-3）

    void Start()
    {
        // 为按钮添加监听
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i; // 重要：创建闭包需要的临时变量
            slotButtons[i].onClick.AddListener(() => OnSlotSelected(index));
        }
        yesButton.onClick.AddListener(OnYesClicked);
        backButton.onClick.AddListener(OnBackClicked);

        // 初始时Yes按钮不可用，直到选中一个存档槽
        yesButton.interactable = false;
    }

    public void SetMode(bool isLoadMode)
    {
        this.isLoadMode = isLoadMode;
        selectedSlotIndex = -1;
        yesButton.interactable = false;
        RefreshSlotUI();
    }

    // 刷新所有存档槽的显示信息
    private void RefreshSlotUI()
    {
        for (int i = 0; i < slotInfoTexts.Length; i++)
        {
            // 这里需要调用SaveLoadManager来获取存档信息
            // string info = SaveLoadManager.Instance.GetSaveSlotInfo(i);
            // slotInfoTexts[i].text = info;

            // 临时示例：
            if (SaveLoadManager.Instance.SaveExists(i))
            {
                slotInfoTexts[i].text = $"Slot {i+1}\nPlayed Time: ..."; // 真实数据需从存档获取
            }
            else
            {
                slotInfoTexts[i].text = "Empty Slot";
            }
        }
    }

    private void OnSlotSelected(int index)
    {
        selectedSlotIndex = index;
        yesButton.interactable = true;

        // 显示确认弹窗
        string message = isLoadMode ? "Load this save?" : "Overwrite this save?";
        FindObjectOfType<MessagePopupController>().ShowPopup(message, OnConfirmAction, OnCancelAction);
    }

    private void OnYesClicked()
    {
        // 再次弹出确认窗口（可选，或者直接执行操作）
        OnSlotSelected(selectedSlotIndex);
    }

    private void OnBackClicked()
    {
        UIManager.Instance.SwitchMenuGroup(UIManager.Instance.startGroup);
    }

    // 当在弹窗中点击“是”
    private void OnConfirmAction()
    {
        if (isLoadMode)
        {
            SaveLoadManager.Instance.LoadGame(selectedSlotIndex);
        }
        else
        {
            SaveLoadManager.Instance.SaveGame(selectedSlotIndex);
            // 然后开始新游戏...
        }
        // 隐藏弹窗
        UIManager.Instance.HideMessagePopup();
    }

    // 当在弹窗中点击“否”
    private void OnCancelAction()
    {
        selectedSlotIndex = -1;
        yesButton.interactable = false;
        UIManager.Instance.HideMessagePopup();
    }
}