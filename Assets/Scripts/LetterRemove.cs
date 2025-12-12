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
    private ArduinoButtonReader arduinoReader;

    void Start()
    {
        arduinoReader = GetComponent<ArduinoButtonReader>();
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

            if (arduinoReader != null)
            {
                arduinoReader.SendLampStateForced(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsAHand(other))
        {
            isHandTouching = false;
            Debug.Log("Hand not touching letter anymore: " + other.gameObject.name);

            if (arduinoReader != null)
            {
                arduinoReader.SendLampStateForced(false);
            }
        }
    }

    void Update()
    {
        bool inputTriggered = Input.GetKeyDown(KeyCode.Space);

        if (arduinoReader != null && arduinoReader.WasButtonPressedThisFrame())
        {
            inputTriggered = true;
        }

        if (inputTriggered && isHandTouching)
        {
            timeOfSpacebarPress = Time.time;
            PerformActionAndDelayReset();
        }
        else if (inputTriggered && !isHandTouching)
        {
            Debug.Log("Input pressed but hand not touching letter!");
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
        if (letterObject != null) letterObject.SetActive(true);
    }

    public float HideLetterAndPlayAudio()
    {
        if (letterObject != null) letterObject.SetActive(false);

        if (resetAudioSource != null && resetClip != null)
        {
            resetAudioSource.PlayOneShot(resetClip);
            return resetClip.length;
        }
        return 0f;
    }
}