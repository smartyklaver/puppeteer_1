using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    public CinematicManager bossManager;

    private Collider swordCollider;

    private void Awake()
    {
        swordCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Dragon"))
            return;

        Debug.Log("⚔️ Sword hit dragon: " + other.name);

        // QTE sword phase
        if (bossManager != null && bossManager.IsSwordHitActive())
        {
            bossManager.RegisterSwordHit();   // ✔ ENIGE plaats waar damage gebeurt (via CMan)

            // knockback en animatie
            DragonController dragon = other.GetComponentInParent<DragonController>();
            if (dragon != null)
                dragon.OnHitByPlayer();

            // SPAM fix
            StartCoroutine(DisableColliderMoment());
            return;
        }

        // tickle phase
        if (bossManager != null && bossManager.IsTickleActive())
        {
            Debug.Log("🫳 Sword hit ignored during tickle phase");
            return;
        }

        Debug.Log("⚠️ Sword hit ignored (not in QTE phase)");
    }

    private IEnumerator DisableColliderMoment()
    {
        swordCollider.enabled = false;
        yield return new WaitForSeconds(0.2f);
        swordCollider.enabled = true;
    }
}
