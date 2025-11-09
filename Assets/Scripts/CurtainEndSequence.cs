using UnityEngine;
using System.Collections;

public class CurtainEndSequence : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth dragonHealth;        // the dragon's health script
    public AudioSource bossMusicSource;     // boss music AudioSource
    public AudioClip victoryMusic;          // victory music clip

    [Header("Curtain Settings")]
    public Transform leftCurtain;
    public Transform rightCurtain;

    // how far curtains move horizontally (8.42 units by default)
    public float curtainShiftX = 8.42f;
    public float closeSpeed = 2f;

    [Header("Audio FX")]
    public AudioSource sfxSource;           // optional: separate sound effect source
    public AudioClip curtainSFX;            // optional: curtain closing sound

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private bool hasClosed = false;

    void Start()
    {
        // store initial positions
        if (leftCurtain != null)
        {
            leftStartPos = leftCurtain.position;
            leftClosedPos = leftStartPos + new Vector3(curtainShiftX, 0f, 0f);
        }
        if (rightCurtain != null)
        {
            rightStartPos = rightCurtain.position;
            rightClosedPos = rightStartPos + new Vector3(-curtainShiftX, 0f, 0f);
        }

        // begin checking for dragon death
        StartCoroutine(WaitForDragonDeath());
    }

    IEnumerator WaitForDragonDeath()
    {
        // wait until the dragonHealth script is destroyed (dragon died)
        while (dragonHealth != null)
            yield return null;

        if (hasClosed) yield break;
        hasClosed = true;

        Debug.Log("🎭 Dragon defeated — closing curtains!");

        // 🔇 Force stop boss music immediately
        if (bossMusicSource != null)
        {
            bossMusicSource.Stop();
            bossMusicSource.clip = null; // clear clip to prevent restarts
        }

        // close the curtains
        yield return StartCoroutine(CloseCurtains());

        // 🏆 play victory music
        if (bossMusicSource != null && victoryMusic != null)
        {
            bossMusicSource.clip = victoryMusic;
            bossMusicSource.loop = false;
            bossMusicSource.volume = 1f;
            bossMusicSource.Play();
        }

        Debug.Log("🏆 Victory music started!");
    }

    IEnumerator CloseCurtains()
    {
        if (sfxSource != null && curtainSFX != null)
            sfxSource.PlayOneShot(curtainSFX);

        bool closing = true;
        while (closing)
        {
            closing = false;

            if (leftCurtain != null && Vector3.Distance(leftCurtain.position, leftClosedPos) > 0.01f)
            {
                leftCurtain.position = Vector3.MoveTowards(leftCurtain.position, leftClosedPos, closeSpeed * Time.deltaTime);
                closing = true;
            }

            if (rightCurtain != null && Vector3.Distance(rightCurtain.position, rightClosedPos) > 0.01f)
            {
                rightCurtain.position = Vector3.MoveTowards(rightCurtain.position, rightClosedPos, closeSpeed * Time.deltaTime);
                closing = true;
            }

            yield return null;
        }

        // snap to exact positions
        if (leftCurtain != null) leftCurtain.position = leftClosedPos;
        if (rightCurtain != null) rightCurtain.position = rightClosedPos;
    }
}
