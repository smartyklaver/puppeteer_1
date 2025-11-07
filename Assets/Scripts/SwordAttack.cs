using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 25f;
    public float attackCooldown = 0.8f;

    [Header("References")]
    public Animator animator; // optioneel voor swing animatie
    public string swingTrigger = "Swing";

    bool canAttack = true;

    void Update()
    {
        // voorbeeld: linkermuisknop voor aanval
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;
        if (animator != null && !string.IsNullOrEmpty(swingTrigger))
            animator.SetTrigger(swingTrigger);

        // reset cooldown
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    // wanneer het zwaard een vijand raakt
    private void OnTriggerEnter(Collider other)
    {
        // enkel damage op vijanden
        if (other.CompareTag("Dragon"))
        {
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log("🗡️ Hit enemy for " + damage + " damage!");
                damageable.TakeDamage(damage);
            }
        }
    }
}
