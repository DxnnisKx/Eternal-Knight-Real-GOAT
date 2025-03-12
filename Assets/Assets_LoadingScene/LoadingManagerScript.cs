using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    [SerializeField] private GameObject loadingScreenObject;
    [SerializeField] private Slider progressBar;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        Debug.Log("LoadingManager started - about to load scene 2");
        LoadScene(2);
    }
    
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
    
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        Debug.Log("Starting to load scene: " + sceneIndex);
        
        // Check for null references
        if (progressBar == null)
        {
            Debug.LogError("Progress bar is null!");
            yield break;
        }
        
        if (loadingScreenObject == null)
        {
            Debug.LogError("Loading screen object is null!");
            yield break;
        }
        
        // Reset progress bar
        progressBar.value = 0;
        
        // Show loading screen
        loadingScreenObject.SetActive(true);
        
        // Wait for at least one frame to ensure the loading screen is visible
        yield return null;
        
        // Start loading the scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        
        // Don't let the scene activate until we allow it
        operation.allowSceneActivation = false;
        
        // While the scene loads
        while (!operation.isDone)
        {
            // Update progress bar (AsyncOperation.progress goes from 0 to 0.9)
            // Normalize to 0-1 range
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            
            // If the load has finished (progress reaches 0.9)
            if (operation.progress >= 0.9f)
            {
                Debug.Log("Load reached 0.9, about to activate scene");
                
                // Force progress bar to 100%
                progressBar.value = 1f;
                
                // Wait a moment to show the completed bar
                yield return new WaitForSeconds(0.5f);
                
                // Activate the scene
                operation.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        Debug.Log("Scene loaded successfully");
        
        // Hide loading screen after transition completes
        loadingScreenObject.SetActive(false);
    }
}