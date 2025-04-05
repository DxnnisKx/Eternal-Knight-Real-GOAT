using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject fpsCounterObject;

    private void Start()
    {
        if (SettingsManager.Instance == null)
        {
            Debug.LogWarning("SettingsManager not found. FPS counter may not work correctly.");
            return;
        }

        if (fpsCounterObject != null)
        {
            fpsCounterObject.SetActive(SettingsManager.Instance.ShowFPSCounter);
        }
    }
}