// IInteractable.cs
// 这个脚本不需要挂载，放在Scripts文件夹即可。
public interface IInteractable
{
    string GetInteractPrompt(); // 返回交互提示的文字，如“交谈”
    void OnInteract(); // 当玩家按下交互键时调用
}