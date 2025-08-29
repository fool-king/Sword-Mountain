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
    
    public void ShowPrompt(IInteractable interactable, Vector3 worldPosition, string actionName = "对话")
    {
        // 如果已有该交互体的提示，先移除
        if (activePrompts.ContainsKey(interactable))
        {
            RemovePrompt(interactable);
        }
        
        // 获取可用的按键
        if (availableKeys.Count == 0)
        {
            Debug.LogWarning("没有可用的交互按键");
            return;
        }
        
        KeyCode assignedKey = availableKeys[0];
        availableKeys.RemoveAt(0);
        
        // 创建提示UI
        GameObject promptObj = Instantiate(promptPrefab, promptContainer);
        InteractionPrompt prompt = promptObj.GetComponent<InteractionPrompt>();
        prompt.Initialize(interactable, assignedKey, actionName, worldPosition);
        
        activePrompts.Add(interactable, prompt);
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