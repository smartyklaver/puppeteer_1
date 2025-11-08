using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 12f;
    public float lifeTime = 12f;

    [Header("Damage")]
    public float damage = 10f;
    public string ownerTag = "Dragon"; // wordt gezet door spawner

    [Header("VFX / SFX")]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Zorg dat deze collider een trigger is
        if (col != null)
            col.isTrigger = true;
    }

    void Start()
    {
        // 🔥 Automatisch vernietigen na X seconden
        Destroy(gameObject, lifeTime);
    }

    // 🔹 Richting en snelheid instellen
    public void Launch(Vector3 direction, float initialSpeed)
    {
        direction.Normalize();
        rb.linearVelocity = direction * initialSpeed;

        // ✅ Zorg dat de fireball NIET botst met zijn eigenaar
        if (!string.IsNullOrEmpty(ownerTag))
        {
            GameObject owner = GameObject.FindGameObjectWithTag(ownerTag);
            if (owner != null)
            {
                Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();
                foreach (var oc in ownerColliders)
                    Physics.IgnoreCollision(col, oc, true);
            }
        }
    }

    // 🔥 Triggers voor damage & vernietiging
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        // 🚫 Negeer collisie met eigenaar
        if (!string.IsNullOrEmpty(ownerTag) && tag == ownerTag)
            return;

        // 🚫 Negeer zwaard (geen effect)
        if (tag == "Sword")
            return;

        // 🚫 Negeer andere fireballs (regen mag elkaar niet raken)
        if (tag == "Fireball")
            return;

        // ✅ Schild → blokkeert en vernietigt
        if (tag == "Shield")
        {
            Explode(other.transform.position);
            return;
        }

        // ✅ Speler → damage + vernietig
        if (tag == "Player")
        {
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log($"🔥 {other.name} takes {damage} damage!");
                damageable.TakeDamage(damage);
            }

            Explode(other.transform.position);
            return;
        }

        // ✅ Grond → vernietig
        if (tag == "Ground")
        {
            Explode(transform.position);
            return;
        }

        // ✅ Muren / plafonds → vernietig
        if (tag == "Wall" || tag == "Ceiling")
        {
            Explode(transform.position);
            return;
        }

        // Andere dingen? Veilig negeren
    }

    // 💥 Explosie en vernietiging
    void Explode(Vector3 at)
    {
        if (hitEffect != null)
            Instantiate(hitEffect, at, Quaternion.identity);

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, at);

        Destroy(gameObject);
    }
}
