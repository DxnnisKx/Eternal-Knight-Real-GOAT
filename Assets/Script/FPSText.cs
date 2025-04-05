using TMPro;
using UnityEngine;

public class FPSText : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private TextMeshProUGUI fpsText;

    void Start()
    {
        fpsText = GetComponent<TextMeshProUGUI>();

        // Debug the preference value when starting
        int showFPS = PlayerPrefs.GetInt("ShowFPS", 0);
        Debug.Log($"Read ShowFPS preference: {showFPS}");

        // Only hide if explicitly disabled
        if (showFPS == 0)
        {
            gameObject.SetActive(false);
            Debug.Log("FPS counter disabled");
        }
        else
        {
            Debug.Log("FPS counter enabled");
        }
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Round(fps)}";
    }
}