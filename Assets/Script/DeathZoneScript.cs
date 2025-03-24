using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private DeathManager deathManager;

    private void Start()
    {
        // Finde den DeathManager in der Szene
        deathManager = FindObjectOfType<DeathManager>();

        if (deathManager == null)
        {
            Debug.LogError("Kein DeathManager in der Szene gefunden!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Überprüfe, ob der Spieler in die Zone eintritt
        if (other.CompareTag("Player"))
        {
            // Optional: Sperre die Bewegung des Spielers
            if (other.GetComponent<PlayerController>() != null)
                other.GetComponent<PlayerController>().enabled = false;

            // Informiere den DeathManager
            if (deathManager != null)
                deathManager.PlayerDied();
        }
    }
}
