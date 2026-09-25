using UnityEngine;

public class ArcherUnit : CombatUnit
{
    private ArcherData archerData;

    [Header("Стрела")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Arrow arrowPrefab;


    protected override float DetectionRange =>
        archerData.DetectionRange;

    protected override float AttackRange =>
        archerData.AttackRange;

    protected override float AttackCooldown =>
        archerData.AttackCooldown;


    protected override void Awake()
    {
        base.Awake();

        archerData = unitData as ArcherData;

        if (archerData == null)
        {
            Debug.LogError(
                $"{name}: UnitData должен быть ArcherData!"
            );
        }
    }


    protected override void Attack()
    {
        if (currentTarget == null)
            return;

        IDamageable target =
            currentTarget.GetComponentInParent<IDamageable>();

        if (target == null)
        {
            currentTarget = null;
            return;
        }

        if (arrowPrefab == null)
        {
            Debug.LogError(
                $"{name}: Arrow Prefab не назначен!"
            );

            return;
        }

        if (shootPoint == null)
        {
            Debug.LogError(
                $"{name}: Shoot Point не назначен!"
            );

            return;
        }

        Debug.Log("Стреляю");

        Arrow arrow = Instantiate(
            arrowPrefab,
            shootPoint.position,
            Quaternion.identity
        );

        arrow.Launch(
            shootPoint.position,
            currentTarget.position,
            archerData.Damage,
            target
        );
    }
}