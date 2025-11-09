using UnityEngine;
using System.Collections;

public class CinematicManager : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;
    public Transform playerTarget;         // spelerpositie om op te focussen
    public Vector3 zoomOffset = new Vector3(0, 1.5f, -2f);
    public float zoomDuration = 3f;
    public float pauseDuration = 2f;

    [Header("Lighting")]
    public Light spotlight;
    public Light2D globalLight2D;          // alleen gebruiken bij 2D URP
    public float darkIntensity = 0.1f;
    public float normalIntensity = 1f;
    public Color bossLightColor = new Color(0.2f, 0.1f, 0.15f); // grim paarsgrijs

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip introMusic;
    public AudioClip bossMusic;
    public AudioClip spotlightSFX;

    [Header("References")]
    public PlayerController player;
    public DragonController dragon;

    private Vector3 startCamPos;
    private Quaternion startCamRot;

    void Start()
    {
        StartCoroutine(StartCinematic());
    }

    IEnumerator StartCinematic()
    {
        // Zorg dat alles geladen is
        yield return new WaitForSeconds(0.2f);

        if (player != null) player.SetCanMove(false);
        if (dragon != null) dragon.enabled = false;

        startCamPos = cameraTransform.position;
        startCamRot = cameraTransform.rotation;

        // Donker maken
        if (globalLight2D != null) globalLight2D.intensity = darkIntensity;
        if (spotlight != null) spotlight.enabled = false;

        // 🎵 Intro muziek
        if (musicSource != null && introMusic != null)
        {
            musicSource.clip = introMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        // 🎥 Zoom in op speler
        Vector3 zoomTarget = playerTarget.position + playerTarget.TransformDirection(zoomOffset);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;
            cameraTransform.position = Vector3.Lerp(startCamPos, zoomTarget, t);
            // Geen rotatie
            yield return null;
        }

        // 🎇 Spotlight + geluid
        if (spotlight != null)
        {
            spotlight.transform.position = playerTarget.position + Vector3.up * 3f;
            spotlight.enabled = true;
        }
        if (spotlightSFX != null)
            AudioSource.PlayClipAtPoint(spotlightSFX, playerTarget.position);

        yield return new WaitForSeconds(pauseDuration);

        // 🎥 Zoom terug naar originele positie
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;
            cameraTransform.position = Vector3.Lerp(zoomTarget, startCamPos, t);
            yield return null;
        }

        // ☠️ Boss fight lighting
        if (globalLight2D != null)
        {
            globalLight2D.intensity = normalIntensity * 0.5f;
            globalLight2D.color = bossLightColor;
        }
        if (spotlight != null)
            spotlight.enabled = false;

        // 🎶 Wissel naar boss muziek
        yield return new WaitForSeconds(0.5f);
        if (musicSource != null && bossMusic != null)
        {
            musicSource.clip = bossMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        // 🐲 Activeer draak en speler
        if (dragon != null) dragon.enabled = true;
        if (player != null) player.SetCanMove(true);

        Debug.Log("🔥 Boss fight started!");
    }
}
