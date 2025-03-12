using UnityEngine;
using TMPro;  // TextMeshPro Namespace hinzufügen

public class MeterDisplay : MonoBehaviour
{
    public TMP_Text meterText;  // TextMeshPro statt normales Text-Element
    public Transform player;  // Der Spieler, dessen Position gemessen wird
    private float startPosX;  // Startposition des Spielers auf der X-Achse

    void Start()
    {
        // Speichert die Startposition des Spielers
        startPosX = player.position.x;
    }

    void Update()
    {
        // Berechnet die Meter relativ zur Startposition
        float meters = player.position.x - startPosX;

        // Zeigt die Meterzahl an, mit 2 Dezimalstellen und verhindert negative Meterzahlen
        meterText.text = Mathf.Max(0, meters).ToString("N0") + " m";
    }
}
