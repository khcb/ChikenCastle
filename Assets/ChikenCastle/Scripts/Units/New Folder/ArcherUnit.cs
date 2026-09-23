using UnityEngine;

public class ArcherUnit : CombatUnit
{
    private ArcherData archerData;
    protected override float DetectionRange => archerData.DetectionRange;
    protected override float AttackRange => archerData.AttackRange;
    protected override float AttackCooldown => archerData.AttackCooldown;
    

    protected override void Awake()
    {
        base.Awake();

        archerData = unitData as ArcherData;
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

        Debug.Log("Стреляю");
        target.TakeDamage(archerData.Damage);
    }
}