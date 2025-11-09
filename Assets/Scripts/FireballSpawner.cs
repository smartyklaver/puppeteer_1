using UnityEngine;
using System.Collections;

public class FireballSpawner : MonoBehaviour
{
    public Transform mouthTransform;
    public GameObject fireballPrefab;
    public float launchSpeed = 14f;
    public float spawnOffset = 0.3f;
    public string shooterTag = "Dragon";
    public Animator animator;
    public string spitTriggerName = "Spit";
    private Coroutine spawnRoutine;


    void OnEnable()
    {
        // start spawning when enabled
        spawnRoutine = StartCoroutine(SpitLoop());
    }

    void OnDisable()
    {
        // stop all coroutines when disabled
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    IEnumerator SpitLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Spit();
            yield return new WaitForSeconds(4);
            
        }
    }

    public void Spit()
    {
        if (fireballPrefab == null || mouthTransform == null) return;

        // Spawn iets voor de mond
        Vector3 spawnPos = mouthTransform.position + mouthTransform.forward * spawnOffset;
        GameObject fbObj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        Fireball fb = fbObj.GetComponent<Fireball>();
        if (fb != null)
        {
            fb.ownerTag = shooterTag;
            Vector3 dir = mouthTransform.forward;
            dir.z = 0f;                       
            dir.Normalize();
            fb.Launch(dir, launchSpeed);

            // 🔥 Belangrijk: negeer collision tussen draak en vuurbal
            Collider[] dragonColliders = GetComponentsInParent<Collider>();
            Collider fbCollider = fb.GetComponent<Collider>();
            foreach (var col in dragonColliders)
            {
                Physics.IgnoreCollision(col, fbCollider, true);
            }
        }

        if (animator != null && !string.IsNullOrEmpty(spitTriggerName))
            animator.SetTrigger(spitTriggerName);
    }
}
