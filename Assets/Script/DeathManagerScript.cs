using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    public GameObject deathScreenUI;
    public string mainMenuSceneName = "MainMenuScene";

    // Diese Methode wird von der DeathZone aufgerufen
    public void PlayerDied()
    {
        // Spiel einfrieren
        Time.timeScale = 0f;

        // Death UI anzeigen
        deathScreenUI.SetActive(true);
    }

    // F�r den Try Again Button
    public void Respawn()
    {
        // Zeit zur�cksetzen
        Time.timeScale = 1f;

        // Aktuelle Szene neu laden
        SceneManager.LoadScene("MainMenuScene");
    }

    // F�r den Main Menu Button
    public void MainMenu()
    {
        // Zeit zur�cksetzen
        Time.timeScale = 1f;

        // Zum Hauptmen�
        SceneManager.LoadScene(mainMenuSceneName);
    }
}