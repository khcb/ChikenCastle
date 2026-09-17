using System;
using UnityEngine;

public class CastleHealth : MonoBehaviour, IDamageable
{
    [Header("Параметры здоровья 🛡️")]
    [SerializeField] private float maxHealth = 100f;

    private float _currentHealth;

    // Передаем float для точности урона
    public event Action<float, float> OnHealthChanged;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"🏰 Замок {gameObject.name} уничтожен!");
        gameObject.SetActive(false);
    }
}