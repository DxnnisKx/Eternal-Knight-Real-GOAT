using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManageToggle : MonoBehaviour
{
    public Toggle showFPSToggle;

    private void Start()
    {
        // Load saved preference
        bool showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        showFPSToggle.isOn = showFPS;
    }

    public void OnToggleChanged()
    {
        PlayerPrefs.SetInt("ShowFPS", showFPSToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}