using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        // Ensure settings panel is hidden at start
        if (settingsMenuPanel != null)
            settingsMenuPanel.SetActive(false);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        // Ensure we have a GameSettingsManager
        if (GameSettingsManager.Instance == null)
        {
            GameObject settingsManager = new GameObject("GameSettingsManager");
            settingsManager.AddComponent<GameSettingsManager>();
        }
    }

    public void PlayGame()
    {
        // Make sure settings are saved before scene transition
        if (GameSettingsManager.Instance != null)
        {
            // This will ensure the setting is stored in PlayerPrefs
            GameSettingsManager.Instance.SetShowFPS(GameSettingsManager.Instance.ShowFPS);
        }
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        if (settingsMenuPanel != null)
            settingsMenuPanel.SetActive(true);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }
}