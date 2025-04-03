using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    // Singleton pattern
    private static GameSettingsManager _instance;
    public static GameSettingsManager Instance { get { return _instance; } }

    // Properties
    private bool _showFPS = false;
    public bool ShowFPS { get { return _showFPS; } }

    // PlayerPrefs keys
    private const string SHOW_FPS_KEY = "ShowFPS";

    private void Awake()
    {
        // Singleton implementation
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved settings
        LoadSettings();
    }

    private void LoadSettings()
    {
        _showFPS = PlayerPrefs.GetInt(SHOW_FPS_KEY, 0) == 1;
    }

    // Method to set and save the ShowFPS setting
    public void SetShowFPS(bool show)
    {
        _showFPS = show;
        PlayerPrefs.SetInt(SHOW_FPS_KEY, show ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Toggle FPS counter
    public void ToggleFPS()
    {
        SetShowFPS(!_showFPS);
    }
}