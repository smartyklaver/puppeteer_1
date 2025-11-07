using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthBarPrefab;
    private Slider healthBarInstance;
    public Vector3 healthBarOffset = new Vector3(0, 3f, 0);

    Camera mainCamera;

    void Start()
    {
        currentHealth = maxHealth;
        mainCamera = Camera.main;

        if (healthBarPrefab != null)
        {
            // spawn direct in wereld
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthBarInstance.maxValue = maxHealth;
            healthBarInstance.value = maxHealth;
        }
        else
        {
            Debug.LogWarning("⚠️ No healthBarPrefab assigned on " + name);
        }
    }

    void Update()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + healthBarOffset;
            healthBarInstance.transform.LookAt(mainCamera.transform);
            healthBarInstance.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"🐉 {name} took {amount} damage! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"💀 {name} died!");
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);
        Destroy(gameObject);
    }
}

