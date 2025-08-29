// InteractableNPC.cs
using UnityEngine;

public class InteractableNPC : MonoBehaviour, IInteractable
{
    
    [Header("基本信息")]
    public string npcName = "村民";
    
    [Header("对话设置")]
    public string[] dialogueLines = { "你好，旅行者！", "需要帮忙吗？" };
    public string[] dialogueOptions = { "有什么任务？", "只是路过", "再见" };
    
    [Header("交互设置")]
    public string promptText = "对话";

    private bool isInteracting = false;

    public string GetInteractPrompt()
    {
        return $"与{npcName}{promptText}";
    }
    
    public void OnInteract()
{
    if (isInteracting) return;
    
    Debug.Log($"与 {npcName} 交互");
    isInteracting = true;
    InteractionManager.Instance.RemovePrompt(this);
    
    AdvancedDialogueManager dialogueManager = GetComponent<AdvancedDialogueManager>();
    if (dialogueManager != null && !dialogueManager.IsInDialogue())
    {
        // 传递this引用给对话管理器
        dialogueManager.StartDialogue(dialogueLines, dialogueOptions, this);
    }
    else
    {
        // 如果没有对话管理器，重置状态
        isInteracting = false;
    }
}

public void OnDialogueEnd()
{
    isInteracting = false;
    Debug.Log($"与 {npcName} 的对话结束");
}
    private void OnTriggerEnter2D(Collider2D other)
   {
    if (other.CompareTag("Player"))
    {
        // 显示"与某某对话"的提示
        InteractionManager.Instance.ShowPrompt(this, transform.position, $"与{npcName}对话");
        
        PlayerInteract playerInteract = other.GetComponent<PlayerInteract>();
        if (playerInteract != null)
        {
            playerInteract.AddNearbyInteractable(this);
        }
    }
   }
    public bool IsInteracting()
{
    return isInteracting;
}

private void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("玩家退出交互范围");
        InteractionManager.Instance.RemovePrompt(this);
        
        PlayerInteract playerInteract = other.GetComponent<PlayerInteract>();
        if (playerInteract != null)
        {
            playerInteract.RemoveNearbyInteractable(this);
        }
        
        // 如果正在对话，强制结束
        // 修改InteractableNPC.cs中的获取方式
        if (isInteracting)
        {
            AdvancedDialogueManager dialogueManager = FindObjectOfType<AdvancedDialogueManager>();
            if (dialogueManager != null)
            {
                dialogueManager.ForceEndDialogue();
            }
            isInteracting = false;
        }
    }
}
}