using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Shield : MonoBehaviour
{
    [Header("Shield Settings")]
    public string shieldOwnerTag = "ShieldOwner"; // optional, not usually needed
    public AudioClip deflectSound;
    public ParticleSystem deflectEffect;
    public CinematicManager cinematicManager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("⚔️ shield hit something: " + other.name);

        // Only if it's the dragon
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth)
        {
            //enemyHealth.TakeDamage(30);

            // IMPORTANT: Register QTE hit
            if (cinematicManager != null)
                cinematicManager.RegisterShieldHit();
        }
    }
}
