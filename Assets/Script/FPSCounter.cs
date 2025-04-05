using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.5f;

    private float accumulatedTime = 0f;
    private int frameCount = 0;
    private float currentFPS = 0f;

    private void Start()
    {
        if (SettingsManager.Instance != null && !SettingsManager.Instance.ShowFPSCounter)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        accumulatedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (accumulatedTime >= updateInterval)
        {
            currentFPS = frameCount / accumulatedTime;

            if (fpsText != null)
            {
                fpsText.text = $"FPS: {Mathf.Round(currentFPS)}";
            }

            frameCount = 0;
            accumulatedTime = 0f;
        }
    }
}