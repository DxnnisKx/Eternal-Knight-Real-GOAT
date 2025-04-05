using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateDeathZonePosition : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SceneAsset scene;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector2(player.transform.position.x, gameObject.transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(scene.name);
        }
    }
}
