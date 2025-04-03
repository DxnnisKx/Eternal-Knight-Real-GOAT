using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSCounter : MonoBehaviour
{
    public Text fpsText;
    public float updateInterval = 0.5f; // How often to update the FPS display

    private float accum = 0.0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeLeft; // Time left for current interval

    void Start()
    {
        if (fpsText == null)
        {
            Debug.LogError("FPSCounter: No Text component assigned!");
            enabled = false;
            return;
        }

        timeLeft = updateInterval;
        DontDestroyOnLoad(this.gameObject); // Keep this object when changing scenes
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        // Update FPS display
        if (timeLeft <= 0.0f)
        {
            float fps = accum / frames;
            string format = System.String.Format("{0:F1} FPS", fps);
            fpsText.text = format;

            // Color-code based on FPS
            if (fps < 30)
                fpsText.color = Color.red;
            else if (fps < 60)
                fpsText.color = Color.yellow;
            else
                fpsText.color = Color.green;

            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
    public void ShowFpsCounter(bool show)
    {
        // Enable/disable the component and the text
        this.enabled = show;

        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(show);
        }
    }
}