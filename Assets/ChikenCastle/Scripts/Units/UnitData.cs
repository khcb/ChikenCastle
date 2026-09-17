using UnityEngine;

public enum UnitType
{
    Melee,     // Ближний бой (Мечник/Воин)
    Ranged,    // Дальний бой (Лучник)
    Gatherer   // Сборщик ресурсов
}

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Тип Юнита 🎭")]
    [SerializeField] private UnitType unitType = UnitType.Melee;

    [Header("Визуал и инфо 🎨")]
    [SerializeField] private string unitName = "Новый Юнит";
    [SerializeField] private Sprite unitIcon;
    [TextArea(2, 4)]
    [SerializeField] private string description;

    [Header("Экономика и Спавн 🪙")]
    [SerializeField] private int cost = 10;
    [SerializeField] private float spawnCooldown = 1f;
    [SerializeField] private GameObject unitPrefab;

    [Header("Характеристики Здоровья 🛡️")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float armor = 0f;

    [Header("Передвижение 🏃")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Боевые параметры ⚔️")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackSpeed = 1f;

    [Header("Параметры Сборщика ⛏️")]
    [SerializeField] private int gatherCapacity = 20;
    [SerializeField] private float gatherSpeed = 1.5f;

    // 🔍 Свойства для чтения (Getters)
    public UnitType Type => unitType;
    public string UnitName => unitName;
    public Sprite UnitIcon => unitIcon;
    public string Description => description;

    public int Cost => cost;
    public float SpawnCooldown => spawnCooldown;
    public GameObject UnitPrefab => unitPrefab;

    public float MaxHealth => maxHealth;
    public float Armor => armor;
    public float MoveSpeed => moveSpeed;

    public float Damage => damage;
    public float AttackRange => attackRange;
    public float AttackSpeed => attackSpeed;

    public int GatherCapacity => gatherCapacity;
    public float GatherSpeed => gatherSpeed;

    /// <summary>
    /// Автоматически срабатывает в редакторе Unity при изменении полей
    /// </summary>
    private void OnValidate()
    {
        switch (unitType)
        {
            case UnitType.Melee:
                unitName = "Мечник";
                cost = 15;
                maxHealth = 120f;
                armor = 2f;
                moveSpeed = 2.5f;
                damage = 20f;
                attackRange = 1.2f;
                attackSpeed = 1f;
                break;

            case UnitType.Ranged:
                unitName = "Лучник";
                cost = 25;
                maxHealth = 70f;
                armor = 0f;
                moveSpeed = 3f;
                damage = 15f;
                attackRange = 6f;
                attackSpeed = 1.5f;
                break;

            case UnitType.Gatherer:
                unitName = "Сборщик";
                cost = 10;
                maxHealth = 50f;
                armor = 0f;
                moveSpeed = 3.5f;
                damage = 0f;
                attackRange = 0f;
                attackSpeed = 0f;
                gatherCapacity = 25;
                gatherSpeed = 1.2f;
                break;
        }
    }
}