using UnityEngine;

[CreateAssetMenu(fileName = "NewWarriorUnit", menuName = "Game/Units/Warrior Unit Data")]
public class WarriorUnitData : UnitData
{
    [Header("Боевые параметры ⚔️")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f; 
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackCooldown =1f;

    public float Damage => damage;
    public float AttackRange => attackRange;
    public float AttackSpeed => attackSpeed;
    public float AttackCooldown => attackCooldown;
    public float DetectionRange => detectionRange;
}