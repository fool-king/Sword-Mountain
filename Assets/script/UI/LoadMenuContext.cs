using UnityEngine;
using System;

public class LoadMenuContext
{
    public bool IsLoadMode { get; set; } // true=载入模式, false=新建模式
    public Action<int> OnConfirmCallback { get; set; } // 确认后的回调函数
    public Action OnCancelCallback { get; set; } // 取消后的回调函数
    public bool AllowOverwrite { get; set; } = true; // 是否允许覆盖存档

    public LoadMenuContext(bool isLoadMode)
    {
        IsLoadMode = isLoadMode;
    }
}