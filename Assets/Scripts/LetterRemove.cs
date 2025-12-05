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

    // NIEUWE VELDEN: Definieer de tags in de Unity Inspector
    [Header("Hand Tags")]
    public string rightHandTag = "RightHand";
    public string leftHandTag = "LeftHand";

    private bool isHandTouching = false;

    // NIEUWE METHODE: Controleert of de collider de juiste tag heeft
    private bool IsAHand(Collider other)
    {
        // Controleert of de collider de RightHand OF de LeftHand tag heeft
        return other.CompareTag(rightHandTag) || other.CompareTag(leftHandTag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsAHand(other)) // Gebruik nu de IsAHand methode
        {
            isHandTouching = true;
            Debug.Log("Hand is touching letter: " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsAHand(other)) // Gebruik nu de IsAHand methode
        {
            isHandTouching = false;
            Debug.Log("Hand not touching letter anymore: " + other.gameObject.name);
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