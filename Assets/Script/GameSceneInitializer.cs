using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    [SerializeField] private FPSCounter fpsDisplay;

    void Start()
    {
        // Find GameSettingsManager
        GameSettingsManager settingsManager = FindObjectOfType<GameSettingsManager>();

        // If it exists, apply the settings
        if (settingsManager != null && fpsDisplay != null)
        {
            fpsDisplay.ShowFpsCounter(settingsManager.ShowFPS);
        }
        else
        {
            Debug.LogWarning("GameSettingsManager not found or FPS display not assigned!");
        }
    }
}