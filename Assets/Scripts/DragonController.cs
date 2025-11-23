using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public FireballSpawner fireballSpawner;
    public Transform head;
    public Camera playerCamera;

    [Header("Movement")]
    public float knockbackForce = 14f;

    [Header("Camera Shake")]
    public float cameraShakeDuration = 0.4f;
    public float cameraShakeIntensity = 0.2f;

    [Header("Roar & Animation")]
    public AudioSource roarAudio;
    public float roarDuration = 2.5f;
    bool isRoaring = false;

    Quaternion headBaseRot;

    void Start()
    {
        if (head != null)
            headBaseRot = head.localRotation;
    }

    // Called by CinematicManager after a hit moment
    public void OnHitByPlayer()
    {
        if (isRoaring) return;

        Debug.Log("⚔️ Dragon hit by player!");
        StartCoroutine(RoarAndReact());
    }

    IEnumerator RoarAndReact()
    {
        isRoaring = true;

        // Knal de speler terug (gebruik player controller)
        if (playerController != null)
        {
            Vector3 dir = (playerController.transform.position - transform.position).normalized;
            dir.y = 0.4f; // small upward arc
            playerController.ApplyKnockback(dir, knockbackForce, 0.8f);
        }

        // Camera shake
        if (playerCamera != null)
            StartCoroutine(CameraShake(cameraShakeDuration, cameraShakeIntensity));

        // Animate head up
        Quaternion targetHeadRot = headBaseRot * Quaternion.Euler(-60f, 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            if (head != null)
                head.localRotation = Quaternion.Slerp(head.localRotation, targetHeadRot, t);
            yield return null;
        }

        // Play roar sound
        if (roarAudio != null)
            roarAudio.Play();

        yield return new WaitForSeconds(roarDuration);

        // Return head
        if (head != null)
            head.localRotation = headBaseRot;

        isRoaring = false;
    }

    // Fireball over player cinematic
public void FireballOverPlayer()
{
    if (fireballSpawner == null)
    {
        Debug.LogError("No FireballSpawner assigned!");
        return;
    }

    fireballSpawner.Spit();
}


    IEnumerator FireballCurve()
    {
        if (fireballSpawner == null || playerController == null){
            yield break;}

        // spawn fireball and get reference
        Fireball fb = fireballSpawner.Spit();
        if (fb == null) yield break;

        GameObject fireball = fb.gameObject;

        Vector3 start = fireball.transform.position;
        Vector3 end = playerController.transform.position + Vector3.up * 2.0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;

            // Lerp + cinematic arc
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * 2f;

            fireball.transform.position = pos;
            yield return null;
        }

        Destroy(fireball);
    }

    // camera shake utility
    IEnumerator CameraShake(float duration, float intensity)
    {
        if (playerCamera == null) yield break;

        Vector3 originalPos = playerCamera.transform.localPosition;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float x = Random.Range(-0.05f, 0.05f) * intensity;
            float y = Random.Range(-0.05f, 0.05f) * intensity;

            playerCamera.transform.localPosition = originalPos + new Vector3(x, y, 0f);

            yield return null;
        }

        playerCamera.transform.localPosition = originalPos;
    }

    public void FireballAtShield()
{
    StartCoroutine(FireballToShield());
}

IEnumerator FireballToShield()
{
    Transform mouth = fireballSpawner.transform;

    GameObject fb = Instantiate(
        fireballSpawner.fireballPrefab,
        mouth.position,
        mouth.rotation
    );

    // GET SHIELD ONLY ONCE
    GameObject shieldObj = GameObject.FindGameObjectWithTag("Shield");
    if (shieldObj == null)
    {
        Debug.LogWarning("No shield found!");
        yield break;
    }

    Transform shield = shieldObj.transform;

    Vector3 start = mouth.position;
    Vector3 end = shield.position;

    float t = 0f;
    while (t < 1f && fb != null)
    {
        t += Time.deltaTime * 2f;
        fb.transform.position = Vector3.Lerp(start, end, t);
        yield return null;
    }

    // If the fireball still exists, destroy it
    if (fb != null)
        Destroy(fb);

    Debug.Log("💥 Fireball hit the shield!");
}


}
