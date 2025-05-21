using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneSkipper : MonoBehaviour
{
    public PlayableDirector timeline;
    public string sceneToLoad;

    public void SkipCutscene()
    {
        timeline.Stop();
        SceneManager.LoadScene("SampleScene");
    }
}
