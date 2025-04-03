using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSscript : MonoBehaviour  // Note: Using the actual class name from your file
{
    [SerializeField] private TextMeshProUGUI fpsText;

    private float updateInterval = 0.5f;
    private float accum = 0;
    private int frames = 0;
    private float timeLeft;

    void Awake()
    {
        if (fpsText == null)
        {
            Debug.LogError("FPSscript: TextMeshProUGUI component not assigned!");
            enabled = false;
            return;
        }

        // Check if GameSettings exists and get the setting
        if (FindObjectOfType<GameSettingsManager>() != null)
        {
            ShowFpsCounter(GameSettingsManager.Instance.ShowFPS);
        }
    }

    void Start()
    {
        timeLeft = updateInterval;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeLeft <= 0.0)
        {
            float fps = accum / frames;
            string format = string.Format("FPS: {0:F1}", fps);
            fpsText.text = format;

            if (fps < 30)
                fpsText.color = Color.red;
            else if (fps < 60)
                fpsText.color = Color.yellow;
            else
                fpsText.color = Color.green;

            timeLeft = updateInterval;
            accum = 0;
            frames = 0;
        }
    }

    // Method to show/hide the FPS counter
    public void ShowFpsCounter(bool show)
    {
        if (fpsText != null)
            fpsText.gameObject.SetActive(show);
    }
}