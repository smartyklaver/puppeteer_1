using UnityEngine;
using System.Collections;

public class CinematicManager : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;
    public Transform playerTarget;
    public Vector3 zoomOffset = new Vector3(0, 2.5f, -2.5f); // hoger en iets verder
    public float zoomDuration = 3f;
    public float pauseDuration = 2f;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip introMusic;
    public AudioClip bossMusic;
    public AudioClip spotlightSFX;

    [Header("References")]
    public PlayerController player;
    public DragonController dragon;
    public FireballSpawner fireballSpawner;

    private Vector3 startCamPos;
    private Quaternion startCamRot;
    private Vector3 zoomPos;

    void Start()
    {
        StartCoroutine(StartCinematic());
    }

    IEnumerator StartCinematic()
    {
        yield return new WaitForSeconds(0.1f);

        if (player != null) player.SetCanMove(false);
        if (dragon != null) dragon.enabled = false;
        if (fireballSpawner != null) fireballSpawner.enabled = false;

        startCamPos = cameraTransform.position;
        startCamRot = cameraTransform.rotation;

        // 🎯 Bereken een iets hogere zoompositie (camera boven speler, kijkt neer)
        zoomPos = playerTarget.position 
                - cameraTransform.forward * 2.5f   // beetje afstand houden
                + Vector3.up ;              

        // 📷 Start ingezoomd en richt op speler
        cameraTransform.position = zoomPos;
        cameraTransform.LookAt(playerTarget.position + Vector3.up * 1.0f); // kijkt iets boven het midden

        // 🎵 Start de intro muziek
        if (musicSource != null && introMusic != null)
        {
            musicSource.clip = introMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        // 💡 Eventueel spotlight geluid
        if (spotlightSFX != null)
            AudioSource.PlayClipAtPoint(spotlightSFX, playerTarget.position);

        // ⏳ Wacht even tijdens ingezoomde scène
        yield return new WaitForSeconds(pauseDuration);

        // 🎥 Zoom soepel terug naar originele positie
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;
            cameraTransform.position = Vector3.Lerp(zoomPos, startCamPos, t);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, startCamRot, t);
            yield return null;
        }

        // 🎶 Wissel naar boss muziek
        yield return new WaitForSeconds(0.3f);
        if (musicSource != null && bossMusic != null)
        {
            musicSource.clip = bossMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        // 🐉 Activeer draak en speler
        if (dragon != null) dragon.enabled = true;
        if (player != null) player.SetCanMove(true);
        if (fireballSpawner != null) fireballSpawner.enabled = true;

        Debug.Log("🔥 Boss fight started!");
    }
}
