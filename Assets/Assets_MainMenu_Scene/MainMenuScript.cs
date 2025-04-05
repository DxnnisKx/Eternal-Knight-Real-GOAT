using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {

        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    // In your settings menu script
    public void SetShowFPS(bool isEnabled)
    {
        PlayerPrefs.SetInt("ShowFPS", isEnabled ? 1 : 0);
        PlayerPrefs.Save(); // Make sure to call Save() to persist the value
        Debug.Log($"ShowFPS set to: {PlayerPrefs.GetInt("ShowFPS")}"); // Add debug logging
    }

}