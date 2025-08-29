using UnityEngine;

public interface IUIPanel
{
    UIPanelType PanelType { get; }
    void OnShow();
    void OnHide();
    bool HandleBackAction(); // 界面特定的返回逻辑
}