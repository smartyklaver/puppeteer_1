using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fireball : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 12f;
    public float lifeTime = 6f;
    public float bounceDamping = 0.9f; // reduce speed on deflect
    public int maxReflections = 3;

    [Header("Damage")]
    public float damage = 10f;
    public string ownerTag = "Player"; // set by spawner so it doesn't hit owner

    [Header("VFX / SFX")]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;

    Rigidbody rb;
    int reflections = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Call this to fire the projectile
    public void Launch(Vector3 direction, float initialSpeed)
    {
        direction = direction.normalized;
        rb.linearVelocity = direction * initialSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        // ignore hitting the owner
        if (!string.IsNullOrEmpty(ownerTag) && other.CompareTag(ownerTag))
            return;

        // If collides with shield -> reflect
        if (other.CompareTag("Shield"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 incoming = rb.linearVelocity.normalized;
            Vector3 reflectDir = Vector3.Reflect(incoming, contact.normal).normalized;

            reflections++;
            if (reflections > maxReflections)
            {
                Explode(contact.point);
                return;
            }

            // optional: change color/emissive on deflect (if you have a Material)
            rb.linearVelocity = reflectDir * rb.linearVelocity.magnitude * bounceDamping;

            // change owner so reflected ball can hit original shooter (optional)
            ownerTag = other.tag; // or set to null to allow hitting anyone

            // small sound
            if (hitSound != null)
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            return;
        }

        // If hits something else (environment, enemy, player):
        // apply damage if the other has a damageable component — otherwise explode
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && (string.IsNullOrEmpty(ownerTag) || !other.CompareTag(ownerTag)))
        {
            damageable.TakeDamage(damage);
        }

        // explode visually
        Explode(collision.contacts[0].point);
    }

    void Explode(Vector3 atPosition)
    {
        if (hitEffect != null)
            Instantiate(hitEffect, atPosition, Quaternion.identity);

        Destroy(gameObject);
    }
}
