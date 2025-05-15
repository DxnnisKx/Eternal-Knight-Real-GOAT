using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject fpsCounterObject;
    [SerializeField] private AudioClip gameMusic;  // Added game music field

    private void Start()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogWarning("SettingsManager not found. FPS counter and music may not work correctly.");
            return;
        }

        // Handle FPS counter visibility
        if (fpsCounterObject != null)
        {
            fpsCounterObject.SetActive(SettingsManager.Instance.ShowFPSCounter);
        }

        // Set up game music if provided
        if (gameMusic != null)
        {
            SettingsManager.Instance.SetMusicClip(gameMusic);
        }
        // The SettingsManager will handle playing or not playing based on user preference
    }
}