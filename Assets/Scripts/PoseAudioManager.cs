using UnityEngine;

public class PoseAudioManager : MonoBehaviour
{
    [Header("🎙️ Story & Feedback Clips")]
    public AudioClip introSpeakers;
    public AudioClip introHeadphones;
    public AudioClip successClip;

    [Header("🎵 Background Music")]
    public AudioClip backgroundClip;
    [Range(0f, 1f)] public float backgroundVolume = 0.05f;

    [Header("Pose Matcher Reference")]
    public PoseMatcher poseMatcher;

    // --- Audio Sources ---
    private AudioSource speakersSource;     // Voor intro via speakers
    private AudioSource headphonesSource;   // Voor intro via koptelefoon
    private AudioSource sharedSource;       // Voor success en achtergrond

    private bool hasPlayedSuccess = false;

    [Header("Output Mode (for testing)")]
    public bool playIntroOnHeadphones = false; // Toggle dit in de Inspector om te testen

    void Start()
    {
        // --- Setup ---
        speakersSource = gameObject.AddComponent<AudioSource>();
        headphonesSource = gameObject.AddComponent<AudioSource>();
        sharedSource = gameObject.AddComponent<AudioSource>();

        // --- Background ---
        sharedSource.volume = backgroundVolume;
        sharedSource.loop = true;

        // --- Start muziek ---
        PlayBackground();

        // --- Kies intro ---
        PlayIntro(playIntroOnHeadphones);
    }

    void Update()
    {
        if (poseMatcher == null) return;

        if (poseMatcher.IsPoseMatched && !hasPlayedSuccess)
        {
            hasPlayedSuccess = true;
            PlaySuccess();
        }
    }

    private void PlayIntro(bool onHeadphones)
    {
        if (onHeadphones)
        {
            if (introHeadphones != null)
            {
                headphonesSource.clip = introHeadphones;
                headphonesSource.loop = false;
                headphonesSource.Play();
                Debug.Log("🎧 Playing intro on headphones");
            }
        }
        else
        {
            if (introSpeakers != null)
            {
                speakersSource.clip = introSpeakers;
                speakersSource.loop = false;
                speakersSource.Play();
                Debug.Log("🔊 Playing intro on speakers");
            }
        }
    }

    private void PlaySuccess()
    {
        if (successClip == null) return;

        sharedSource.Stop();
        sharedSource.loop = false;
        sharedSource.volume = 1f;
        sharedSource.clip = successClip;
        sharedSource.Play();
        Debug.Log("✅ Success sound playing on both outputs");
    }

    private void PlayBackground()
    {
        if (backgroundClip == null) return;

        sharedSource.clip = backgroundClip;
        sharedSource.loop = true;
        sharedSource.volume = backgroundVolume;
        sharedSource.Play();
        Debug.Log("🎵 Background music started");
    }
}
