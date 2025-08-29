using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeyConfigController : MonoBehaviour
{
    [System.Serializable]
    public class KeyBinding
    {
        public string actionName;
        public KeyCode defaultKey;
        public KeyCode currentKey;
        public Button bindingButton;
        public TMP_Text buttonText;
    }

    public KeyBinding[] keyBindings;

    public Button resetDefaultsButton;
    public Button backButton;

    private bool isWaitingForKey = false;
    private KeyBinding currentBindingToChange;

    private void Start()
    {
        LoadKeyBindings();
        SetupButtonListeners();
    }

    private void LoadKeyBindings()
    {
        foreach (KeyBinding binding in keyBindings)
        {
            // 从PlayerPrefs加载保存的按键，如果没有则使用默认值
            int savedKey = PlayerPrefs.GetInt(binding.actionName, (int)binding.defaultKey);
            binding.currentKey = (KeyCode)savedKey;
            binding.buttonText.text = binding.currentKey.ToString();
        }
    }

    private void SetupButtonListeners()
    {
        // 为每个按键绑定按钮添加监听
        foreach (KeyBinding binding in keyBindings)
        {
            binding.bindingButton.onClick.AddListener(() => StartKeyRebinding(binding));
        }

        resetDefaultsButton.onClick.AddListener(ResetToDefaults);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void StartKeyRebinding(KeyBinding binding)
    {
        if (isWaitingForKey) return;

        isWaitingForKey = true;
        currentBindingToChange = binding;
        binding.buttonText.text = "按下任意键...";
        
        StartCoroutine(WaitForKeyPress());
    }

    private IEnumerator WaitForKeyPress()
    {
        while (isWaitingForKey)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // 避免绑定某些系统按键
                        if (keyCode != KeyCode.Escape && keyCode != KeyCode.Mouse0)
                        {
                            currentBindingToChange.currentKey = keyCode;
                            currentBindingToChange.buttonText.text = keyCode.ToString();
                            
                            // 保存到PlayerPrefs
                            PlayerPrefs.SetInt(currentBindingToChange.actionName, (int)keyCode);
                            
                            // 这里可以调用一个全局的按键映射更新方法
                            // InputManager.Instance.UpdateKeyBinding(currentBindingToChange.actionName, keyCode);
                        }
                        break;
                    }
                }
                isWaitingForKey = false;
                currentBindingToChange = null;
                yield break;
            }
            yield return null;
        }
    }

    private void ResetToDefaults()
    {
        foreach (KeyBinding binding in keyBindings)
        {
            binding.currentKey = binding.defaultKey;
            binding.buttonText.text = binding.defaultKey.ToString();
            
            // 保存默认值到PlayerPrefs
            PlayerPrefs.SetInt(binding.actionName, (int)binding.defaultKey);
            
            // 更新全局按键映射
            // InputManager.Instance.UpdateKeyBinding(binding.actionName, binding.defaultKey);
        }
    }

    private void OnBackButtonClicked()
    {
        gameObject.SetActive(false); // 关闭按键设置面板
        InGameMenuManager.Instance.settingsPanel.SetActive(true); // 返回设置面板
    }

    private void OnGUI()
    {
        if (isWaitingForKey)
        {
            // 显示等待按键输入的提示
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), 
                     "按下任意键绑定...\n(ESC取消)", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 20 });
        }
    }
}