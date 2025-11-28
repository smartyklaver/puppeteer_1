using UnityEngine;
using UnityEngine.Events;
using System.Collections; // Nodig voor Coroutine

public class LetterRemove : MonoBehaviour
{
    public GameObject letterObject;
    public UnityEvent OnResetReady = new UnityEvent();

    [Header("Audio Delay Settings")]
    public AudioSource resetAudioSource;
    public AudioClip resetClip;
    public CameraSwitcher cameraSwitcher;

    [HideInInspector]
    public float timeOfSpacebarPress = -1f;

    private bool isHandTouching = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            isHandTouching = true;
            Debug.Log("Hand is touching letter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            isHandTouching = false;
            Debug.Log("Hand not touching letter anymore");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isHandTouching)
        {
            timeOfSpacebarPress = Time.time;
            PerformActionAndDelayReset();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isHandTouching)
        {
            Debug.Log("Spacebar pressed but hand not touching letter!");
        }
    }

    public void PerformActionAndDelayReset()
    {
        SignalResetComplete();
    }

    private void SignalResetComplete()
    {
        //send to TimelineRestarter 
        OnResetReady.Invoke();
        cameraSwitcher.SwitchCameraDisplays();
    }

    public void ShowLetter()
    {
        if (letterObject != null)
        {
            letterObject.SetActive(true);
        }
    }

    public float HideLetterAndPlayAudio()
    {
        if (letterObject != null)
        {
            letterObject.SetActive(false);
        }

        if (resetAudioSource != null && resetClip != null)
        {
            resetAudioSource.PlayOneShot(resetClip);
            return resetClip.length;
        }

        return 0f;
    }
}