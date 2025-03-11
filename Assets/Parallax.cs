using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;               // Kamera, die die Bewegung verursacht
    public float parallexEffect;         // Der Effekt (geschwindigkeit) des Parallax-Effekts

    void Start()
    {
        startPos = transform.position.x;  // Startposition des Hintergrunds
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Berechnet die Breite des Hintergrunds
    }

    void Update()
    {
        // Berechnung der Bewegung des Hintergrunds basierend auf der Kamera
        float temp = (cam.transform.position.x * (1 - parallexEffect));
        float dist = (cam.transform.position.x * parallexEffect);

        // Setze die Position des Hintergrunds
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // Wenn der Hintergrund das Ende überschreitet, wird die Startposition zurückgesetzt
        if (temp > startPos + length)
        {
            startPos += length;  // Hintergrund zurücksetzen nach rechts
        }
        else if (temp < startPos - length)
        {
            startPos -= length;  // Hintergrund zurücksetzen nach links
        }
    }
}
