using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextAppearAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    //private float delay = 2;

    private void Start()
    {
        //for (float i = 0; i < 1; i += 0.1f)
        //{
        //    Invoke("SetTextTransparency", delay);
        //    //StartCoroutine(SetTextTransparency(i));
        //}
    }

    private void SetTextTransparency(float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
