// 消息弹窗控制器
public class MessagePopupController : MonoBehaviour, IUIPanel
{
    public UIManager.UIPanelType PanelType => UIManager.UIPanelType.MessagePopup;
    
    [SerializeField] private TMPro.TextMeshProUGUI messageText;
    [SerializeField] private UnityEngine.UI.Button confirmButton;
    [SerializeField] private UnityEngine.UI.Button cancelButton;
    
    private System.Action onConfirm;
    private System.Action onCancel;
    
    public void Setup(string message, System.Action confirmAction = null, System.Action cancelAction = null)
    {
        messageText.text = message;
        onConfirm = confirmAction;
        onCancel = cancelAction;
        
        confirmButton.gameObject.SetActive(confirmAction != null || cancelAction == null);
        cancelButton.gameObject.SetActive(cancelAction != null);
    }
    
    public void OnShow()
    {
        gameObject.SetActive(true);
        // 可以添加显示动画等效果
    }
    
    public void OnHide()
    {
        gameObject.SetActive(false);
        // 清理回调，避免内存泄漏
        onConfirm = null;
        onCancel = null;
    }
    
    public bool HandleBackAction()
    {
        OnCancel();
        return true;
    }
    
    public void OnConfirm()
    {
        onConfirm?.Invoke();
        UIManager.Instance.HideMessagePopup();
    }
    
    public void OnCancel()
    {
        onCancel?.Invoke();
        UIManager.Instance.HideMessagePopup();
    }
}