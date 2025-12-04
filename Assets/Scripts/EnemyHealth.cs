// EnemyHealth.cs
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float current;

    [Header("UI")]
    public Slider healthBar; // optional existing slider

    public System.Action OnEnemyDied;
    void Start()
    {
        current = maxHealth;
        if (healthBar != null) { healthBar.maxValue = maxHealth; healthBar.value = current; }
    }

    public void TakeDamage(float amount)
    {
        current = Mathf.Clamp(current - amount, 0f, maxHealth);
        Debug.Log($"{name} took {amount} damage! ({current}/{maxHealth})");
        if (healthBar != null) healthBar.value = current;
        if (current <= 0) Die();
    }

    // For QTE small hits where you want smaller behaviour
    public void TakeQTEHit(float amount)
    {
        TakeDamage(amount);
    }

    void Die()
    {
        Debug.Log($"{name} died!");
        OnEnemyDied?.Invoke();
        gameObject.SetActive(false);
    }
    public void ResetHealth()
    {
        current = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = current;
        }
    }

}
