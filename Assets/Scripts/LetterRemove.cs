using UnityEngine;
using UnityEngine.Events;

public class InputRemover : MonoBehaviour
{
    // --- Letter/Signal Components ---
    public GameObject letterObject;
    // NOTE: We change this event name for clarity to reflect the delay:
    public UnityEvent OnResetReady = new UnityEvent();

    // --- Audio Components (NEW) ---
    [Header("Audio Delay Settings")]
    public AudioSource resetAudioSource; // ?? Drag an AudioSource component here
    public AudioClip resetClip;         // ?? Drag the sound file here

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Call the function that handles the delay
            PerformActionAndDelayReset();
        }
    }

    public void PerformActionAndDelayReset()
    {
        // 1. Hide the letter immediately
        if (letterObject != null)
        {
            letterObject.SetActive(false);
            Debug.Log("Letter removed.");
        }

        // 2. Play the sound and schedule the reset
        if (resetAudioSource != null && resetClip != null)
        {
            resetAudioSource.PlayOneShot(resetClip);

            // Invoke the final signal after the duration of the audio clip.
            // This is the pause!
            Invoke(nameof(SignalResetComplete), resetClip.length);
        }
        else
        {
            // 3. Fail-safe: If audio components are missing, reset immediately
            Debug.LogWarning("Reset Audio is missing or incomplete. Resetting immediately.");
            OnResetReady.Invoke();
        }
    }

    // 4. This function is called by Invoke() after the sound has finished playing
    private void SignalResetComplete()
    {
        // Send the signal to the TimelineRestarter
        OnResetReady.Invoke();
    }
}