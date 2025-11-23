
using UnityEngine;

using UnityEngine.Events;

using System.Collections; // Nieuwe import voor Coroutine



public class LetterRemove : MonoBehaviour

{

    public GameObject letterObject;

    public UnityEvent OnResetReady = new UnityEvent();



    [Header("Audio Delay Settings")]

    public AudioSource resetAudioSource;

    public AudioClip resetClip;



    // Nieuw: Slaat de absolute tijd op waarop de spatiebalk werd ingedrukt

    [HideInInspector]

    public float timeOfSpacebarPress = -1f;



    void Update()

    {

        if (Input.GetKeyDown(KeyCode.Space))

        {

            // Vang de tijd van de druk op de spatiebalk op

            timeOfSpacebarPress = Time.time;

            PerformActionAndDelayReset();

        }

    }



    public void PerformActionAndDelayReset()

    {
        SignalResetComplete();

    }



    private void SignalResetComplete()

    {

        // Stuurt het signaal naar de TimelineRestarter (die de replay start)

        OnResetReady.Invoke();

    }



    // NIEUW: Functie om de letter zichtbaar te maken bij de start van de replay

    public void ShowLetter()

    {

        if (letterObject != null)

        {

            letterObject.SetActive(true);

        }

    }



    // NIEUW: Functie om de letter onzichtbaar te maken na de replay

    public float HideLetterAndPlayAudio()

    {

        if (letterObject != null)

        {

            letterObject.SetActive(false);

        }
        resetAudioSource.PlayOneShot(resetClip);
        return resetClip.length;

    }

}