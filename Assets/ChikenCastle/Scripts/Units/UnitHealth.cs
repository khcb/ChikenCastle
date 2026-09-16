using UnityEngine;

public class UnitHealth : MonoBehaviour, IDamageable
{
    [Header("Параметры здоровья")]
    [SerializeField] private float maxHealth = 20f;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log($"⚔️ Юнит {gameObject.name} получил {amount} урона! Осталось HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"💀 Юнит {gameObject.name} погиб!");
        Destroy(gameObject);
    }
}