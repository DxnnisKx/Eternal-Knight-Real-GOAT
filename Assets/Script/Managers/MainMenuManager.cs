using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] public Toggle fpsToggle;
    [SerializeField] public Toggle musicToggle; // Added music toggle
    [SerializeField] private AudioClip menuMusic; // Music for the menu

    private void Start()
    {
        if (SettingsManager.Instance == null)
        {
            GameObject settingsObject = new GameObject("SettingsManager");
            settingsObject.AddComponent<SettingsManager>();
        }

        // Set up menu music if provided
        if (menuMusic != null)
        {
            SettingsManager.Instance.SetMusicClip(menuMusic);
        }

        if (fpsToggle != null)
        {
            fpsToggle.isOn = SettingsManager.Instance.ShowFPSCounter;
            fpsToggle.onValueChanged.AddListener(OnFPSToggleChanged);
        }

        // Set up music toggle
        if (musicToggle != null)
        {
            musicToggle.isOn = SettingsManager.Instance.PlayMusic;
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }
    }

    private void OnFPSToggleChanged(bool isOn)
    {
        SettingsManager.Instance.SetFPSCounterVisibility(isOn);
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        SettingsManager.Instance.SetMusicEnabled(isOn);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}