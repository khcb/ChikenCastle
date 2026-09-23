using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Units/Base Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Визуал и инфо")]
    [SerializeField] private string unitName = "Юнит";
    [SerializeField] private Sprite unitIcon;

    [Header("Экономика и Спавн")]
    [SerializeField] private int cost = 10;
    [SerializeField] private float spawnCooldown = 1f;
    [SerializeField] private GameObject unitPrefab;

    [Header("Характеристики")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 3f;

    // Геттеры
    public string UnitName => unitName;
    public Sprite UnitIcon => unitIcon;
    public int Cost => cost;
    public float SpawnCooldown => spawnCooldown;
    public GameObject UnitPrefab => unitPrefab;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
}