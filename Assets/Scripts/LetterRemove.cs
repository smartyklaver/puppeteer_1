using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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

    [Header("Hand Tags")]
    public string rightHandTag = "RightHand";
    public string leftHandTag = "LeftHand";

    private bool isHandTouching = false;

    // We make this private because we will find it automatically
    private ArduinoButtonReader arduinoReader;

    void Start()
    {
        // Automatically find the ArduinoButtonReader on THIS same object
        arduinoReader = GetComponent<ArduinoButtonReader>();

        if (arduinoReader == null)
        {
            Debug.LogError("Error: No 'ArduinoButtonReader' script found on this object! Please add it.");
        }
    }

    private bool IsAHand(Collider other)
    {
        return other.CompareTag(rightHandTag) || other.CompareTag(leftHandTag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsAHand(other))
        {
            isHandTouching = true;
            Debug.Log("Hand is touching letter: " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsAHand(other))
        {
            isHandTouching = false;
            Debug.Log("Hand not touching letter anymore: " + other.gameObject.name);
        }
    }

    void Update()
    {
        // 1. Default to Spacebar
        bool inputTriggered = Input.GetKeyDown(KeyCode.Space);

        // 2. Check Arduino if the script was found
        if (arduinoReader != null)
        {
            if (arduinoReader.WasButtonPressedThisFrame())
            {
                inputTriggered = true;
            }
        }

        // 3. Final Logic
        if (inputTriggered && isHandTouching)
        {
            Debug.Log("button pressed and Hand is touching!");
            timeOfSpacebarPress = Time.time;
            PerformActionAndDelayReset();
        }
        else if (inputTriggered && !isHandTouching)
        {
            Debug.Log("button pressed but hand not touching letter!");
        }
    }

    public void PerformActionAndDelayReset()
    {
        SignalResetComplete();
    }

    private void SignalResetComplete()
    {
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