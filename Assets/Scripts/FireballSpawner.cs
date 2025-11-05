using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public Transform mouthTransform;         // set to the bone/transform of the mouth
    public GameObject fireballPrefab;
    public float launchSpeed = 14f;
    public float spawnOffset = 0.3f;         // push out of the mouth slightly
    public string shooterTag = "Player";     // tag used as ownerTag

    public Animator animator;                // optional: play spit animation
    public string spitTriggerName = "Spit";  // optional

    public void Spit()
    {
        if (fireballPrefab == null || mouthTransform == null) return;

        Vector3 spawnPos = mouthTransform.position + mouthTransform.forward * spawnOffset;
        GameObject fbObj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        // align forward
        fbObj.transform.forward = mouthTransform.forward;

        Fireball fb = fbObj.GetComponent<Fireball>();
        if (fb != null)
        {
            fb.ownerTag = shooterTag;
            fb.Launch(mouthTransform.forward, launchSpeed);
        }

        if (animator != null && !string.IsNullOrEmpty(spitTriggerName))
            animator.SetTrigger(spitTriggerName);
    }

    // Example: call Spit() on key press
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Spit();
    }
}
