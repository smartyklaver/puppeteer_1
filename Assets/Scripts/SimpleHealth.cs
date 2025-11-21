using UnityEngine;
using UnityEngine.UI;

public class SimpleHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthBar; // drag je UI Slider hier in de Inspector

    [Header("Effects")]
    public AudioClip hurtSound;
    public ParticleSystem hurtEffect;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"{gameObject.name} takes {amount} damage!");
        currentHealth -= amount;

        if (hurtEffect != null)
            Instantiate(hurtEffect, transform.position, Quaternion.identity);

        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);

        UpdateHealthUI();

       // if (currentHealth <= 0)
           // Die();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        // tijdelijke death reactie
        gameObject.SetActive(false); 
    }
}
