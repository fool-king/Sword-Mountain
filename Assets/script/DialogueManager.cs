// AdvancedDialogueManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AdvancedDialogueManager : MonoBehaviour
{
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;
    public Transform optionsContainer;
    public GameObject optionPrefab;
    public TMP_InputField inputField;
    public GameObject inputPanel;
    
    private List<DialogueOption> currentOptions = new List<DialogueOption>();
    private int selectedOptionIndex = 0;
    private Coroutine typingCoroutine;
    private System.Action<string> onInputSubmit;
    
    void Start()
    {
        dialogueUI.SetActive(false);
        inputPanel.SetActive(false);
    }
    
    public void StartDialogue(string[] dialogueLines, string[] options = null)
    {
        dialogueUI.SetActive(true);
        StartCoroutine(DisplayDialogue(dialogueLines, options));
    }
    
    private IEnumerator DisplayDialogue(string[] lines, string[] options)
    {
        foreach (string line in lines)
        {
            // 显示对话文本
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(line));
            
            // 等待玩家点击继续
            bool continued = false;
            while (!continued)
            {
                if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
                {
                    continued = true;
                }
                yield return null;
            }
        }
        
        // 显示选项（如果有）
        if (options != null && options.Length > 0)
        {
            ShowOptions(options);
        }
        else
        {
            EndDialogue();
        }
    }
    
    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }
    
    private void ShowOptions(string[] options)
    {
        ClearOptions();
        
        for (int i = 0; i < options.Length; i++)
        {
            GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
            DialogueOption option = optionObj.GetComponent<DialogueOption>();
            int index = i;
            
            option.Initialize(options[i], () => SelectOption(index));
            currentOptions.Add(option);
        }
        
        selectedOptionIndex = 0;
        UpdateOptionSelection();
    }
    
    public void ShowInputField(System.Action<string> onSubmit)
    {
        onInputSubmit = onSubmit;
        inputPanel.SetActive(true);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }
    
    public void OnInputSubmit()
    {
        string inputText = inputField.text;
        inputPanel.SetActive(false);
        onInputSubmit?.Invoke(inputText);
    }
    
    private void Update()
    {
        // 选项导航
        if (currentOptions.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedOptionIndex = (selectedOptionIndex - 1 + currentOptions.Count) % currentOptions.Count;
                UpdateOptionSelection();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedOptionIndex = (selectedOptionIndex + 1) % currentOptions.Count;
                UpdateOptionSelection();
            }
            else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Return))
            {
                SelectOption(selectedOptionIndex);
            }
        }
    }
    
    private void UpdateOptionSelection()
    {
        for (int i = 0; i < currentOptions.Count; i++)
        {
            currentOptions[i].SetSelected(i == selectedOptionIndex);
        }
    }
    
    private void SelectOption(int index)
    {
        // 处理选项选择
        Debug.Log($"选择了选项: {index}");
        ClearOptions();
        EndDialogue();
    }
    
    private void ClearOptions()
    {
        foreach (DialogueOption option in currentOptions)
        {
            Destroy(option.gameObject);
        }
        currentOptions.Clear();
    }
    
    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
        ClearOptions();
    }
    
    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}