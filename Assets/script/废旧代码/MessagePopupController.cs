using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessagePopupController : MonoBehaviour
{
    public TMP_Text messageText;
    public Button confirmButton;
    public Button cancelButton;

    // 用于存储确认和取消后的回调函数
    private System.Action onConfirm;
    private System.Action onCancel;

    public void ShowPopup(string message, System.Action confirmCallback, System.Action cancelCallback)
    {
        messageText.text = message;
        onConfirm = confirmCallback;
        onCancel = cancelCallback;

        gameObject.SetActive(true); // 显示弹窗

        // 为按钮添加一次性的监听
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnConfirm()
    {
        onConfirm?.Invoke(); // 调用确认回调
        gameObject.SetActive(false); // 隐藏自己
    }

    private void OnCancel()
    {
        onCancel?.Invoke(); // 调用取消回调
        gameObject.SetActive(false); // 隐藏自己
    }
}