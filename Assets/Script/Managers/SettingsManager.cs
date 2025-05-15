using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public bool ShowFPSCounter { get; private set; }
    public bool PlayMusic { get; private set; } = false; // Default to false - music off by default

    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create an AudioSource if not assigned in inspector
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = 0.5f; // Default volume at 50%
            }

            // Initialize music based on current setting
            UpdateMusicState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetFPSCounterVisibility(bool isVisible)
    {
        ShowFPSCounter = isVisible;
    }

    public void SetMusicEnabled(bool isEnabled)
    {
        PlayMusic = isEnabled;
        UpdateMusicState();
    }

    private void UpdateMusicState()
    {
        if (musicSource != null)
        {
            if (PlayMusic)
            {
                // Only play if we have a clip and aren't already playing
                if (musicSource.clip != null && !musicSource.isPlaying)
                {
                    musicSource.Play();
                }
            }
            else
            {
                // Stop if we're currently playing
                if (musicSource.isPlaying)
                {
                    musicSource.Stop();
                }
            }
        }
    }

    // Call this method to set the music clip to play
    public void SetMusicClip(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            // Only change clip if it's different
            if (musicSource.clip != clip)
            {
                bool wasPlaying = musicSource.isPlaying;

                // Stop current playback
                if (wasPlaying)
                {
                    musicSource.Stop();
                }

                // Set new clip
                musicSource.clip = clip;

                // Resume playback if we should be playing
                if (PlayMusic)
                {
                    musicSource.Play();
                }
            }
        }
    }
}