// Editor/CopyAllObjectNames.cs
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

public class CopyAllObjectNames : EditorWindow
{
    [MenuItem("Tools/复制所有对象名称")]
    public static void ShowWindow()
    {
        GetWindow<CopyAllObjectNames>("复制对象名称");
    }

    void OnGUI()
    {
        if (GUILayout.Button("复制场景中所有对象名称"))
        {
            CopySceneObjectNames();
        }

        if (GUILayout.Button("复制选中对象名称"))
        {
            CopySelectedObjectNames();
        }

        if (GUILayout.Button("复制特定类型的对象名称"))
        {
            CopySpecificTypeObjectNames();
        }
    }

    void CopySceneObjectNames()
    {
        StringBuilder sb = new StringBuilder();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.transform.parent == null) // 只复制根对象
            {
                sb.AppendLine(obj.name);
                AppendChildrenNames(obj.transform, sb, 1);
            }
        }
        
        CopyToClipboard(sb.ToString(), "场景中所有对象名称");
    }

    void AppendChildrenNames(Transform parent, StringBuilder sb, int depth)
    {
        foreach (Transform child in parent)
        {
            string indent = new string(' ', depth * 2);
            sb.AppendLine($"{indent}└─ {child.name}");
            AppendChildrenNames(child, sb, depth + 1);
        }
    }

    void CopySelectedObjectNames()
    {
        if (Selection.gameObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "请先选择一些对象", "确定");
            return;
        }

        StringBuilder sb = new StringBuilder();
        foreach (GameObject obj in Selection.gameObjects)
        {
            sb.AppendLine(obj.name);
        }
        
        CopyToClipboard(sb.ToString(), "选中对象名称");
    }

    void CopySpecificTypeObjectNames()
    {
        StringBuilder sb = new StringBuilder();
        
        // 复制所有NPC名称
        InteractableNPC[] npcs = FindObjectsOfType<InteractableNPC>();
        sb.AppendLine("=== NPC列表 ===");
        foreach (InteractableNPC npc in npcs)
        {
            sb.AppendLine(npc.name);
        }

        // 复制所有对话管理器
        AdvancedDialogueManager[] dialogueManagers = FindObjectsOfType<AdvancedDialogueManager>();
        sb.AppendLine("\n=== 对话管理器列表 ===");
        foreach (AdvancedDialogueManager manager in dialogueManagers)
        {
            sb.AppendLine(manager.name);
        }

        CopyToClipboard(sb.ToString(), "特定类型对象名称");
    }

    void CopyToClipboard(string text, string message)
    {
        GUIUtility.systemCopyBuffer = text;
        EditorUtility.DisplayDialog("成功", $"{message}已复制到剪贴板", "确定");
        Debug.Log($"已复制到剪贴板:\n{text}");
    }
}