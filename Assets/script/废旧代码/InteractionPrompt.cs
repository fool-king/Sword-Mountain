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

    void Awake()
    {
        // 确保获取组件引用
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void SetupPrompt(IInteractable interactable, KeyCode key, string actionName, Vector3 worldPosition)
    {
        targetInteractable = interactable;
    interactionKey = key;

    keyText.text = key.ToString();
    actionText.text = actionName;

    // 固定位置在NPC上方
    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
    // 固定在NPC上方一定距离
    transform.position = screenPos + new Vector3(0, 80, 0); // 垂直上方80像素
    
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
        LeanTween.alphaCanvas(canvasGroup, 0, 0.3f).setOnComplete(() =>
        {
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