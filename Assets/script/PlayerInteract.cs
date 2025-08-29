// PlayerInteract.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 添加这行
using UnityEngine.InputSystem.Interactions; // 可选添加

public class PlayerInteract : MonoBehaviour
{
    public GameObject interactPromptUI;
    public Text interactText;
    
    private PlayerInput playerInput;
    private InputAction interactAction;
    private IInteractable currentInteractable;

    void Start()
    {
        // 获取PlayerInput组件
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            // 如果物体上没有PlayerInput，尝试查找或添加
            playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("场景中没有PlayerInput组件！");
                return;
            }
        }
        
        // 获取Interact动作
        interactAction = playerInput.actions.FindAction("Interact");
        if (interactAction == null)
        {
            Debug.LogError("在Input Actions中找不到Interact动作！");
            return;
        }
        
        Debug.Log("Input System初始化完成");
        
        if (interactPromptUI != null) 
            interactPromptUI.SetActive(false);
    }

    void Update()
    {
        // Input System会自动处理输入，这里不需要Update检测
        // 但保留其他逻辑
    }

    // 使用Input System的事件回调
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) // 确保是按键按下而不是释放
        {
            Debug.Log("交互键被按下");
            if (currentInteractable != null)
            {
                currentInteractable.OnInteract();
            }
            else
            {
                Debug.Log("当前没有可交互对象");
            }
        }
    }

    public void AddNearbyInteractable(IInteractable interactable)
    {
        Debug.Log($"添加可交互对象: {interactable}");
        currentInteractable = interactable;
        
        if (interactPromptUI != null) 
            interactPromptUI.SetActive(true);
        if (interactText != null) 
            interactText.text = $"[E] {interactable.GetInteractPrompt()}";
    }

    public void RemoveNearbyInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable)
        {
            Debug.Log("移除可交互对象");
            currentInteractable = null;
            if (interactPromptUI != null) 
                interactPromptUI.SetActive(false);
        }
    }
}