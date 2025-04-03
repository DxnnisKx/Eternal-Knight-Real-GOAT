using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Toggle showFPSToggle;

    private void OnEnable()
    {
        // Initialize toggle based on current settings when menu opens
        if (showFPSToggle != null && GameSettingsManager.Instance != null)
        {
            showFPSToggle.isOn = GameSettingsManager.Instance.ShowFPS;
        }
    }

    private void Start()
    {
        // Set up listener for toggle
        if (showFPSToggle != null)
        {
            showFPSToggle.onValueChanged.AddListener(OnShowFPSToggleChanged);
        }
    }

    private void OnShowFPSToggleChanged(bool isOn)
    {
        if (GameSettingsManager.Instance != null)
        {
            GameSettingsManager.Instance.SetShowFPS(isOn);
        }
    }

    public void BackToMainMenu()
    {
        // Hide settings menu
        gameObject.SetActive(false);

        // Show main menu
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
}