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

    // Für den Try Again Button
    public void Respawn()
    {
        // Zeit zurücksetzen
        Time.timeScale = 1f;

        // Aktuelle Szene neu laden
        SceneManager.LoadScene("MainMenuScene");
    }

    // Für den Main Menu Button
    public void MainMenu()
    {
        // Zeit zurücksetzen
        Time.timeScale = 1f;

        // Zum Hauptmenü
        SceneManager.LoadScene(mainMenuSceneName);
    }
}