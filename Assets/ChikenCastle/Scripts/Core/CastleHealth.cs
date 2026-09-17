using System;
using UnityEngine;

public class CastleHealth : MonoBehaviour, IDamageable
{
    [Header("Параметры здоровья 🛡️")]
    [SerializeField] private float maxHealth = 100f;

    private float _currentHealth;
    private bool _isDead = false; // 👈 Защита от повторного вызова Die()

    public event Action<float, float> OnHealthChanged;
    public event Action<CastleHealth> OnCastleDestroyed;

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
        if (_isDead) return; // Если уже уничтожен, дальше код не идет

        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true; // Фиксируем смерть

        Debug.Log($"🏰 Замок {gameObject.name} уничтожен!");
        
        OnCastleDestroyed?.Invoke(this); // Вызываем событие

        gameObject.SetActive(false); // Выключаем замок
    }
}