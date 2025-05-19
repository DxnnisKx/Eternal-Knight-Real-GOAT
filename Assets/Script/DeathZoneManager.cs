using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;
using System;

public class UpdateDeathZonePosition : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SceneAsset scene;
    // Update is called once per frame
    private MeterDisplay meterDisplay; // Reference to MeterDisplay component

    private void Start()
    {
        // Get reference to the MeterDisplay component
        meterDisplay = FindFirstObjectByType<MeterDisplay>();

        // Alternative approach: if MeterDisplay is on the player object
        // meterDisplay = player.GetComponent<MeterDisplay>();
    }
    void Update()
    {
        gameObject.transform.position = new Vector2(player.transform.position.x, gameObject.transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            int currentScore = 0;
            if (meterDisplay != null)
            {
                currentScore = meterDisplay.meterInt;
            }
            else
            {
                Debug.LogError("MeterDisplay component not found!");
            }
            SceneManager.LoadScene(scene.name);
            string server = "192.168.231.159"; // IP deines Raspberry Pi
            string database = "EK_HighScores";
            string user = "pi";
            string password = "raspberry";

            string connectionString = $"Server={server};Database={database};User ID={user};Password={password};";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO scores (score) VALUES (@score)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@score", currentScore);  // Beispielwert
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Datensatz erfolgreich eingefügt!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler: " + ex.Message);
            }
        }
         //Delete Enemies on touch
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}
