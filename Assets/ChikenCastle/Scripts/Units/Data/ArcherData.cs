using UnityEngine;

[CreateAssetMenu(fileName = "NewArcherUnit", menuName = "Game/Units/Arche Unit Data")]
public class ArcherData: UnitData
{
    [Header("Боевые параметры ⚔️")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float detectionRange  = 5f; 
    [SerializeField] private float attackRange = 3f; 
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackCooldown =1f;

    public float Damage => damage;
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public float AttackSpeed => attackSpeed;
    public float AttackCooldown => attackCooldown;
}
