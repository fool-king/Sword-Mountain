// DialogueOption.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueOption : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI optionText;
    public Image background;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.gray;
    public Color selectedColor = Color.blue;
    
    private System.Action onClick;
    private bool isSelected = false;
    
    public void Initialize(string text, System.Action callback)
    {
        optionText.text = text;
        onClick = callback;
        background.color = normalColor;
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        background.color = selected ? selectedColor : normalColor;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            background.color = hoverColor;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            background.color = normalColor;
        }
    }
}