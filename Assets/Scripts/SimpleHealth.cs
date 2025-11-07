using UnityEngine;

public class SimpleHealth : MonoBehaviour, IDamageable
{
    public float hp = 50f;

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
