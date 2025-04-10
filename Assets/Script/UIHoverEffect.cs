using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    private TextMeshProUGUI tmpText;
    private Toggle toggle;
    private Image checkmark;
    private void Start()
    {
        // Check if this is directly on a TMP text
        tmpText = GetComponent<TextMeshProUGUI>();
        // If not found, look for it in children (for buttons and toggles)
        if (tmpText == null)
            tmpText = GetComponentInChildren<TextMeshProUGUI>();
        // For toggles, try to find the checkmark image
        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            Transform checkmarkTransform = transform.Find("Background/Checkmark");
            if (checkmarkTransform != null)
                checkmark = checkmarkTransform.GetComponent<Image>();
        }
        // Set initial color
        if (tmpText != null)
            tmpText.color = normalColor;
        if (checkmark != null)
            checkmark.color = normalColor;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmpText != null)
            tmpText.color = hoverColor;
        if (checkmark != null)
            checkmark.color = hoverColor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmpText != null)
            tmpText.color = normalColor;
        if (checkmark != null)
            checkmark.color = normalColor;
    }
}