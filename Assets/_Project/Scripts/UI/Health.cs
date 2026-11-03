
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Points de vie")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    // Application des degats
    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        Debug.Log($"{gameObject.name} : {currentHealth}/{maxHealth} PV");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Restauration de vie
    public void Heal(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        Debug.Log($"{gameObject.name} : soin applique {currentHealth}/{maxHealth} PV");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Gestion de la mort
    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"{gameObject.name} est mort");

        OnDeath?.Invoke();
    }

    // Reset de la vie
    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
