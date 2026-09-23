using System;
using UnityEngine;

public class CastleHealth : MonoBehaviour, IDamageable
{
    [Header("Здоровье")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Команда")]
    [SerializeField] private Team team;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public Team Team => team;

    public event Action<float, float> OnHealthChanged;
    public event Action<CastleHealth> OnCastleDestroyed;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"Замок {gameObject.name} уничтожен.");

        OnCastleDestroyed?.Invoke(this);

        gameObject.SetActive(false);
    }
}