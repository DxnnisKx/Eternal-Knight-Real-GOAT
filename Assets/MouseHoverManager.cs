using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MouseHoverManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Button btn;
    public SceneAsset scene;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(scene.name);
    }
    public void OnPointerEnter(PointerEventData eventData)
    { 
        var color = btn.colors.normalColor;
        color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var color = btn.colors.normalColor;
        color = Color.white;
    }
}