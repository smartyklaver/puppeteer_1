using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fireball : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 12f;
    public float lifeTime = 12f;
    public float bounceDamping = 0.9f;
    public int maxReflections = 3;

    [Header("Damage")]
    public float damage = 10f;
    public string ownerTag = "Dragon"; // set by spawner

    [Header("VFX / SFX")]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;

    Rigidbody rb;
    int reflections = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Zorg dat de fireball zijn eigen colliders negeert
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        if (allColliders.Length > 1)
        {
            for (int i = 1; i < allColliders.Length; i++)
                Physics.IgnoreCollision(allColliders[0], allColliders[i]);
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 direction, float initialSpeed)
    {
        direction.Normalize();
        rb.linearVelocity = direction * initialSpeed; // ✅ juiste veld gebruiken
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore hitting the owner
        if (!string.IsNullOrEmpty(ownerTag) && other.CompareTag(ownerTag))
            return;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log($"🔥 {other.name} takes {damage} damage!");
            damageable.TakeDamage(damage);
        }

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
