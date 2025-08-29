//道具数据结构
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemID;
    public string itemName;
    public string description;
    public Sprite icon = null; // 添加默认值
    public int quantity;
    public bool isUsable = true;
    public string effectScriptName; // 关联的效果脚本名称
    public string category = "General"; // 添加分类
    public int qualityLevel = 1; // 添加品质等级
}