using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private CinematicManager cinematicManager;

    private void Start()
    {
        // Find the cinematic manager in the scene
        cinematicManager = FindObjectOfType<CinematicManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("⚔️ Sword hit something: " + other.name);

        // Only if it's the dragon
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth)
        {
            //enemyHealth.TakeDamage(5);

            // IMPORTANT: Register QTE hit
            if (cinematicManager != null)
                cinematicManager.RegisterSwordHit();
        }
    }
}
