// DialogueUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Button continueButton;
    public float typingSpeed = 0.05f; // 打字速度

    private string[] currentLines;
    private int currentLineIndex;
    private Coroutine typingCoroutine;

    void Start()
    {
        // 初始时隐藏对话框
        gameObject.SetActive(false);
        // 绑定按钮事件
        continueButton.onClick.AddListener(ShowNextSentence);
    }

    public void StartDialogue(string[] lines)
    {
        gameObject.SetActive(true);
        currentLines = lines;
        currentLineIndex = 0;
        ShowSentence(0);
    }

    private void ShowSentence(int index)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeSentence(currentLines[index]));
    }

    // 打字机效果协程
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    public void ShowNextSentence()
    {
        currentLineIndex++;
        if (currentLineIndex < currentLines.Length)
        {
            ShowSentence(currentLineIndex);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        gameObject.SetActive(false);
        // 通知DialogueManager对话结束
        
    }
}