using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    [Header("Setup")]
    public Transform mouthTransform;
    public GameObject fireballPrefab;
    public float launchSpeed = 14f;
    public float spawnOffset = 0.3f;
    public string shooterTag = "Dragon";

    // Spawns and RETURNS the Fireball so Dragon can animate it
    public Fireball Spit()
    {
        if (fireballPrefab == null || mouthTransform == null)
            return null;

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

            // dragon doesn't collide with its own projectile
            GameObject owner = GameObject.FindGameObjectWithTag(shooterTag);
            if (owner != null)
            {
                Collider[] ownerCols = owner.GetComponentsInChildren<Collider>();
                Collider fbCol = fbObj.GetComponent<Collider>();
                if (fbCol != null)
                {
                    foreach (var c in ownerCols)
                        Physics.IgnoreCollision(c, fbCol, true);
                }
            }
        }

        return fb;
    }

    // Utility for cinematic resets
    public void DespawnAllFireballs()
    {
        Fireball[] all = FindObjectsOfType<Fireball>();
        foreach (var f in all)
            Destroy(f.gameObject);
    }
}
