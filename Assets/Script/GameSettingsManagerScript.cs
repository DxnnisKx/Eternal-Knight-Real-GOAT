using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance { get; private set; }

    // Setting for displaying FPS
    public bool ShowFPS { get; private set; } = false;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to toggle FPS display setting
    public void SetShowFPS(bool showFPS)
    {
        ShowFPS = showFPS;

        // Find and update any active FPS displays
        FPSscript[] displays = FindObjectsOfType<FPSscript>();
        foreach (var display in displays)
        {
            display.ShowFpsCounter(ShowFPS);
        }
    }
}