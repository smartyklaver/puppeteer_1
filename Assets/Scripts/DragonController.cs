using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public FireballSpawner fireballSpawner;
    public Transform head;
    public Camera playerCamera;
    public bool FireballLaunched = false;


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

    
public void FireballOverPlayer()
{
    if (fireballSpawner == null)
    {
        Debug.LogError("No FireballSpawner assigned!");
        return;
    }
    FireballLaunched = true;

    fireballSpawner.Spit();
    FireballLaunched = false;

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

   

}
