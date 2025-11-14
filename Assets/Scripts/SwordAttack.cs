using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 5f;


    

    // wanneer het zwaard een vijand raakt
    private void OnTriggerEnter(Collider other)
{
    // enkel damage op vijanden
    if (other.CompareTag("Dragon"))
    {
        Debug.Log("⚔️ Sword hit dragon: " + other.name);

        // 🔥 1️⃣ Probeer DragonController direct aan te roepen voor brul/knockback
        DragonController dragon = other.GetComponentInParent<DragonController>();
        if (dragon != null)
        {
            Debug.Log("🐉 Triggering dragon reaction (OnHitByPlayer)");
            dragon.OnHitByPlayer();
        }
        else
        {
            Debug.LogWarning("⚠️ Dragon hit but no DragonController found!");
        }

        // 💥 2️⃣ Pas schade toe via IDamageable (voor health vermindering)
        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log($"🗡️ Hit enemy for {damage} damage!");
            damageable.TakeDamage(damage);
        }
    }
}

}
