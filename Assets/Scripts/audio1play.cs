using UnityEngine;

public class PlayAudioDelayed : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        Invoke("PlaySound", 0f); // 3 seconds delay
    }

    void PlaySound()
    {
        audioSource.Play();
    }
}
