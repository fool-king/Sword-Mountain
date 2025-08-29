// InteractionPrompt.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InteractionPrompt : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI actionText;
    public Image background;
    public KeyCode interactionKey;
    
    private IInteractable targetInteractable;
    private CanvasGroup canvasGroup;
    
    public void Initialize(IInteractable interactable, KeyCode key, string actionName, Vector3 worldPosition)
    {
        targetInteractable = interactable;
        interactionKey = key;
        
        keyText.text = key.ToString();
        actionText.text = actionName;
        
        // 更新位置（世界坐标转屏幕坐标）
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPos + new Vector3(100, 50, 0); // 右上偏移
        
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        LeanTween.alphaCanvas(canvasGroup, 1, 0.3f);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        TriggerInteraction();
    }
    
    public void TriggerInteraction()
    {
        targetInteractable?.OnInteract();
        Hide();
    }
    
    public void Hide()
    {
        LeanTween.alphaCanvas(canvasGroup, 0, 0.3f).setOnComplete(() => {
            Destroy(gameObject);
        });
    }
    
    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            TriggerInteraction();
        }
    }
}