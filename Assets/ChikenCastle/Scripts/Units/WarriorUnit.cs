using UnityEngine;

public class WarriorUnit : CombatUnit
{
    private WarriorUnitData warriorData;
    protected override float DetectionRange => warriorData.DetectionRange;
    protected override float AttackRange => warriorData.AttackRange;
    protected override float AttackCooldown => warriorData.AttackCooldown;
    
    protected override void Awake()
    {
        base.Awake();

        warriorData = unitData as WarriorUnitData;
    }

    protected override void Attack()
    {
        if (currentTarget == null)
            return;

        IDamageable target = currentTarget.GetComponentInParent<IDamageable>();

        if (target == null)
        {
            currentTarget = null;
            return;
        }

        Debug.Log("Атакую");
        target.TakeDamage(warriorData.Damage);
    }
}
