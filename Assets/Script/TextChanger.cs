using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TextChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color normalTextColor = Color.black;
    public Color hoverTextColor = Color.red;

    private Text legacyText;
    private TextMeshProUGUI tmpText;
    private bool usingTMP;

    void Start()
    {
        // Check which text component is being used
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            usingTMP = true;
            tmpText.color = normalTextColor;
        }
        else
        {
            legacyText = GetComponentInChildren<Text>();
            if (legacyText != null)
            {
                legacyText.color = normalTextColor;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (usingTMP && tmpText != null)
        {
            tmpText.color = hoverTextColor;
        }
        else if (legacyText != null)
        {
            legacyText.color = hoverTextColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (usingTMP && tmpText != null)
        {
            tmpText.color = normalTextColor;
        }
        else if (legacyText != null)
        {
            legacyText.color = normalTextColor;
        }
    }
}