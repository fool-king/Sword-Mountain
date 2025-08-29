// AdvancedDialogueManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class AdvancedDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;
    public Transform optionsContainer;
    public GameObject optionPrefab;
    public TMP_InputField inputField;
    public GameObject inputPanel;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public KeyCode skipDialogueKey = KeyCode.Escape;

    private List<DialogueOption> currentOptions = new List<DialogueOption>();
    private int selectedOptionIndex = 0;
    private Coroutine typingCoroutine;
    private System.Action<string> onInputSubmit;
    private bool isInDialogue = false;
    private PlayerMovement playerMovement;
    private InteractableNPC currentNPC;
    private bool referencesInitialized = false;

    public static AdvancedDialogueManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("多个AdvancedDialogueManager实例，销毁重复的: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        EnsureReferences();
    }

    void Start()
    {
        EnsureReferences();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (!isInDialogue) return;

        // 按ESC跳过对话
        if (Input.GetKeyDown(skipDialogueKey))
        {
            ForceEndDialogue();
        }

        // 选项导航（如果有选项）
        HandleOptionNavigation();
    }

    void EnsureReferences()
    {
        if (referencesInitialized) return;

        Debug.Log($"[AdvancedDialogueManager] 初始化引用 - {gameObject.name}");

        // 确保dialogueUI引用
        if (dialogueUI == null)
        {
            dialogueUI = GameObject.Find("DialogueUI");
            if (dialogueUI == null)
            {
                Debug.LogError("无法找到DialogueUI对象！请在场景中创建或手动赋值。");
                return;
            }
        }

        // 确保其他UI组件引用
        if (dialogueText == null)
        {
            dialogueText = dialogueUI.transform.Find("DialogueText")?.GetComponent<TextMeshProUGUI>();
            if (dialogueText == null) Debug.LogWarning("无法找到DialogueText组件");
        }

        if (optionsContainer == null)
        {
            optionsContainer = dialogueUI.transform.Find("OptionsContainer")?.GetComponent<Transform>();
            if (optionsContainer == null) Debug.LogWarning("无法找到OptionsContainer");
        }

        if (inputPanel == null)
        {
            inputPanel = dialogueUI.transform.Find("InputPanel")?.GetComponent<GameObject>();
            if (inputPanel == null) Debug.LogWarning("无法找到InputPanel");
        }

        if (inputField == null && inputPanel != null)
        {
            inputField = inputPanel.transform.Find("InputField")?.GetComponent<TMP_InputField>();
            if (inputField == null) Debug.LogWarning("无法找到InputField");
        }

        // 设置初始UI状态
        if (dialogueUI != null) dialogueUI.SetActive(false);
        if (inputPanel != null) inputPanel.SetActive(false);

        referencesInitialized = true;
        Debug.Log($"[AdvancedDialogueManager] 引用初始化完成 - {gameObject.name}");
    }

    public void StartDialogue(string[] dialogueLines, string[] options = null, InteractableNPC npc = null)
    {
        if (isInDialogue || !referencesInitialized) return;

        Debug.Log("开始对话");
        isInDialogue = true;
        currentNPC = npc;
        
        if (dialogueUI != null) dialogueUI.SetActive(true);

        // 禁止玩家移动
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        StartCoroutine(DisplayDialogueCoroutine(dialogueLines, options));
    }

    private IEnumerator DisplayDialogueCoroutine(string[] lines, string[] options)
    {
        foreach (string line in lines)
        {
            // 显示当前对话行
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(line));

            // 等待玩家点击继续
            bool continued = false;
            while (!continued && isInDialogue)
            {
                if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
                {
                    continued = true;
                }
                yield return null;
            }

            // 如果对话被强制结束，跳出循环
            if (!isInDialogue) break;
        }

        // 显示选项或结束对话
        if (isInDialogue)
        {
            if (options != null && options.Length > 0)
            {
                ShowOptions(options);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private IEnumerator TypeText(string text)
    {
        if (dialogueText == null) yield break;

        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            if (!isInDialogue) yield break; // 如果对话结束，停止打字
            
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void ShowOptions(string[] options)
    {
        if (optionsContainer == null || optionPrefab == null) return;

        ClearOptions();
        
        for (int i = 0; i < options.Length; i++)
        {
            int optionIndex = i;
            GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
            DialogueOption dialogueOption = optionObj.GetComponent<DialogueOption>();
            
            if (dialogueOption != null)
            {
                dialogueOption.Initialize(options[optionIndex], () => OnOptionSelected(optionIndex));
                currentOptions.Add(dialogueOption);
            }
        }
        
        if (currentOptions.Count > 0)
        {
            selectedOptionIndex = 0;
            currentOptions[selectedOptionIndex].SetSelected(true);
        }
    }

    private void HandleOptionNavigation()
    {
        if (currentOptions.Count == 0) return;

        // 键盘导航
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectOption((selectedOptionIndex + 1) % currentOptions.Count);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectOption((selectedOptionIndex - 1 + currentOptions.Count) % currentOptions.Count);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnOptionSelected(selectedOptionIndex);
        }
    }

    private void SelectOption(int index)
    {
        if (currentOptions.Count == 0) return;

        currentOptions[selectedOptionIndex].SetSelected(false);
        selectedOptionIndex = index;
        currentOptions[selectedOptionIndex].SetSelected(true);
    }

    private void OnOptionSelected(int index)
    {
        Debug.Log("选择了选项: " + index);
        // 这里可以添加选项处理逻辑
        EndDialogue();
    }

    private void ClearOptions()
    {
        foreach (DialogueOption option in currentOptions)
        {
            if (option != null && option.gameObject != null)
            {
                Destroy(option.gameObject);
            }
        }
        currentOptions.Clear();
        selectedOptionIndex = 0;
    }

    public void EndDialogue()
    {
        if (!isInDialogue) return;

        Debug.Log("结束对话");
        isInDialogue = false;
        
        if (dialogueUI != null) dialogueUI.SetActive(false);
        ClearOptions();
        
        if (inputPanel != null) inputPanel.SetActive(false);

        // 通知NPC对话结束
        if (currentNPC != null)
        {
            currentNPC.OnDialogueEnd();
            currentNPC = null;
        }

        // 恢复玩家移动
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
    }

    public void ForceEndDialogue()
    {
        EndDialogue();
    }

    public bool IsInDialogue()
    {
        return isInDialogue;
    }

    private bool IsPointerOverUIElement()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        
        // 检查触摸输入
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
            {
                return true;
            }
        }
        
        return false;
    }

    // 输入框相关方法
    public void ShowInputPanel(System.Action<string> onSubmit)
    {
        if (inputPanel == null) return;

        onInputSubmit = onSubmit;
        inputPanel.SetActive(true);
        
        if (inputField != null)
        {
            inputField.text = "";
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    public void OnInputSubmit()
    {
        if (inputField != null && !string.IsNullOrEmpty(inputField.text))
        {
            onInputSubmit?.Invoke(inputField.text);
            inputPanel.SetActive(false);
        }
    }

    // 调试方法
    public void PrintReferences()
    {
        Debug.Log($"dialogueUI: {dialogueUI != null}");
        Debug.Log($"dialogueText: {dialogueText != null}");
        Debug.Log($"optionsContainer: {optionsContainer != null}");
        Debug.Log($"optionPrefab: {optionPrefab != null}");
        Debug.Log($"inputPanel: {inputPanel != null}");
        Debug.Log($"inputField: {inputField != null}");
    }
}