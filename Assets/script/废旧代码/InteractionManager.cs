// InteractionManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;
    
    public GameObject promptPrefab;
    public Transform promptContainer;
    
    private Dictionary<IInteractable, InteractionPrompt> activePrompts = new Dictionary<IInteractable, InteractionPrompt>();
    private List<KeyCode> availableKeys = new List<KeyCode> { KeyCode.F, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V };
    
    void Awake()
    {
        Instance = this;
    }
    
    // InteractionManager.cs - 修改ShowPrompt方法
    public void ShowPrompt(IInteractable interactable, Vector3 worldPosition, string actionName = "对话")
    {
        // 检查是否已经在显示该提示
        if (activePrompts.ContainsKey(interactable))
        {
            // 更新位置
            activePrompts[interactable].transform.position = 
                Camera.main.WorldToScreenPoint(worldPosition) + new Vector3(100, 50, 0);
            return;
        }
        
        if (availableKeys.Count == 0) return;
        
        KeyCode assignedKey = availableKeys[0];
        availableKeys.RemoveAt(0);
        
        GameObject promptObj = Instantiate(promptPrefab, promptContainer);
        InteractionPrompt prompt = promptObj.GetComponent<InteractionPrompt>();
        
        if (prompt != null)
        {
            prompt.SetupPrompt(interactable, assignedKey, actionName, worldPosition);
            activePrompts.Add(interactable, prompt);
        }
    }
    
    public void RemovePrompt(IInteractable interactable)
{
    if (activePrompts.ContainsKey(interactable))
    {
        availableKeys.Add(activePrompts[interactable].interactionKey);
        activePrompts[interactable].Hide();
        activePrompts.Remove(interactable);
    }
}

public void ShowPromptForNPC(InteractableNPC npc)
{
    // 如果NPC不在对话中，显示提示
    if (!npc.IsInteracting())
    {
        ShowPrompt(npc, npc.transform.position, $"与{npc.npcName}对话");
    }
}
    
    public void ClearAllPrompts()
    {
        foreach (var prompt in activePrompts.Values.ToList())
        {
            prompt.Hide();
        }
        activePrompts.Clear();
        availableKeys = new List<KeyCode> { KeyCode.F, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V };
    }
}