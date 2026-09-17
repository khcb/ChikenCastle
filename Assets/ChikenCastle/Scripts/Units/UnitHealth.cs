using UnityEngine;

public class UnitHealth : MonoBehaviour, IDamageable
{
    [Header("Данные юнита (Scriptable Object) 📄")]
    [SerializeField] private UnitData unitData;

    private float _currentHealth;
    private float _armor;

    private void Awake()
    {
        // Если данные уже назначены в префабе
        if (unitData != null)
        {
            Init(unitData);
        }
    }

    /// <summary>
    /// Инициализация здоровья напрямую из ScriptableObject (вызывается при спавне)
    /// </summary>
    public void Init(UnitData data)
    {
        unitData = data;
        _armor = unitData.Armor;
        _currentHealth = unitData.MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        // Учитываем броню (урон не может быть меньше 1)
        float finalDamage = Mathf.Max(1f, amount - _armor);
        _currentHealth -= finalDamage;

        Debug.Log($"⚔️ {gameObject.name} получил {finalDamage} урона! HP: {_currentHealth}/{unitData.MaxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"💀 {gameObject.name} погиб!");
        Destroy(gameObject);
    }
}