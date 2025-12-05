using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 40f;
    public float lifeTime = 12f;

    [Header("Damage")]
    public float damage = 10f;
    public string ownerTag = "Dragon"; // wordt gezet door spawner

    [Header("VFX / SFX")]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;

    private Rigidbody rb;
    private Collider col;
    private const float fixedZ = -2.33f;

   void Awake()
{
    rb = GetComponent<Rigidbody>();
    col = GetComponent<Collider>();

    if (rb == null)
        rb = gameObject.AddComponent<Rigidbody>();

    if (col == null)
        col = gameObject.AddComponent<SphereCollider>();

    // 🚀 Belangrijk: physics uitschakelen, alleen triggers gebruiken
    rb.useGravity = false;
    rb.isKinematic = false;
    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    rb.interpolation = RigidbodyInterpolation.Interpolate;

    // 🔒 Zorg dat het ALTIJD een trigger is
    col.isTrigger = true;
    col.enabled = true;

    // 🔧 Fysische afstoting vermijden
    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
}

void Start()
{
    // 🔥 Automatisch vernietigen na X seconden
    Destroy(gameObject, lifeTime);

    // 🚫 Negeer botsingen tussen vuurballen (veiligheidsnet)
    Fireball[] others = FindObjectsOfType<Fireball>();
    foreach (var other in others)
    {
        if (other != this && other.col != null && col != null)
            Physics.IgnoreCollision(col, other.col);
    }
}


    // 🔹 Richting en snelheid instellen
public void Launch(Vector3 direction, float initialSpeed)
    {
        direction.Normalize();
        direction.z = 0; // 🔹 Zorg dat richting nooit Z bevat
        rb.linearVelocity = direction * initialSpeed;

        if (!string.IsNullOrEmpty(ownerTag))
        {
            GameObject owner = GameObject.FindGameObjectWithTag(ownerTag);
            if (owner != null)
            {
                Collider[] ownerCols = owner.GetComponentsInChildren<Collider>();
                foreach (var oc in ownerCols)
                    Physics.IgnoreCollision(col, oc, true);
            }
        }
    }

    void Update()
    {
        // 🔹 Houd altijd vaste Z-positie
        if (Mathf.Abs(transform.position.z - fixedZ) > 0.001f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, fixedZ);
        }

        // 🔹 Verwijder ongewenste Z-beweging
        if (rb.linearVelocity.z != 0)
        {
            Vector3 vel = rb.linearVelocity;
            vel.z = 0;
            rb.linearVelocity = vel;
        }
    }

    // 🔥 Triggers voor damage & vernietiging
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        // 🚫 Negeer collisie met eigenaar
        if (tag == "Dragon")
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
