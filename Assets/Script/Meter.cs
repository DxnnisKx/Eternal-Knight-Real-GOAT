using UnityEngine;
using TMPro;  // TextMeshPro Namespace hinzufügen

public class MeterDisplay : MonoBehaviour
{
    public TMP_Text meterText;  // TextMeshPro statt normales Text-Element
    public int meterInt;
    public Transform player;  // Der Spieler, dessen Position gemessen wird
    private int startPosX;  // Startposition des Spielers auf der X-Achse

    void Start()
    {
        // Speichert die Startposition des Spielers
        startPosX = (int)player.position.x;
    }

    void Update()
    {
        // Berechnet die Meter relativ zur Startposition
        int meters = (int)player.position.x - startPosX;
        meterInt = meters;

        // Zeigt die Meterzahl an, mit 2 Dezimalstellen und verhindert negative Meterzahlen
        meterText.text = Mathf.Max(0, meters).ToString("N0") + " m";
    }
}
