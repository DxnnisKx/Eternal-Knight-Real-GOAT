using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject fpsCounterPrefab; // Prefab with FPSCounter and Text components
    public Toggle showFpsToggle; // Reference to the toggle in the settings menu

    private static SettingsManager _instance;
    private GameObject fpsCounterInstance;

    public static SettingsManager Instance
    {
        get { return _instance; }
        
    }

    void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Load settings on startup
        LoadSettings();
    }

    void Start()
    {
        // Set up the toggle listener
        if (showFpsToggle != null)
        {
            showFpsToggle.onValueChanged.AddListener(OnFpsToggleChanged);

            // Initialize toggle state based on saved preference
            showFpsToggle.isOn = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        }
    }

    void LoadSettings()
    {
        // Load the show FPS setting
        bool showFps = PlayerPrefs.GetInt("ShowFPS", 0) == 1;

        // Apply the setting
        SetFpsCounterActive(showFps);
    }

    public void OnFpsToggleChanged(bool isOn)
    {
        // Save the setting
        PlayerPrefs.SetInt("ShowFPS", isOn ? 1 : 0);
        PlayerPrefs.Save();

        // Apply the setting
        SetFpsCounterActive(isOn);
    }

    void SetFpsCounterActive(bool active)
    {
        if (active && fpsCounterInstance == null)
        {
            // Instantiate the FPS counter
            fpsCounterInstance = Instantiate(fpsCounterPrefab);
        }
        else if (!active && fpsCounterInstance != null)
        {
            // Destroy the FPS counter
            Destroy(fpsCounterInstance);
            fpsCounterInstance = null;
        }

        // If the counter exists, ensure its state is correct
        if (fpsCounterInstance != null)
        {
            fpsCounterInstance.SetActive(active);
        }
    }
}