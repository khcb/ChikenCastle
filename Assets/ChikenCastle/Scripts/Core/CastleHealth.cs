using UnityEngine;

public class CastleHealth : MonoBehaviour, IDamageable
{
    [Header("Параметры здоровья")]
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        Debug.Log($"Замок получил {amount} урона! Осталось HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Замок уничтожен! Победа!");
        Destroy(gameObject);
    }
}