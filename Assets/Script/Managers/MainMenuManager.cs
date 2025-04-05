using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] public Toggle fpsToggle;

    private void Start()
    {
        if (SettingsManager.Instance == null)
        {
            GameObject settingsObject = new GameObject("SettingsManager");
            settingsObject.AddComponent<SettingsManager>();
        }

        if (fpsToggle != null)
        {
            fpsToggle.isOn = SettingsManager.Instance.ShowFPSCounter;
            fpsToggle.onValueChanged.AddListener(OnFPSToggleChanged);
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

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}