// InteractableNPC.cs
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("玩家进入交互范围");
        // 显示交互提示
        InteractionManager.Instance.ShowPrompt(this, transform.position, "对话");
    }
}

private void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("玩家退出交互范围");
        // 移除交互提示
        InteractionManager.Instance.RemovePrompt(this);
    }
}

public void OnInteract()
{
    Debug.Log("NPC交互触发");
    // 移除提示
    InteractionManager.Instance.RemovePrompt(this);
    
    // 开始对话
    GetComponent<AdvancedDialogueManager>()?.StartDialogue(
        new string[] { "你好，旅行者！", "今天天气真不错。" },
        new string[] { "选项1：询问任务", "选项2：交易", "选项3：输入文本" }
    );
}