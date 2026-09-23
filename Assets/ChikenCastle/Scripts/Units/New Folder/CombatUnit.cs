using UnityEngine;

public abstract class CombatUnit : UnitBase
{
    protected float attackTimer;

    protected abstract float DetectionRange { get; }
    protected abstract float AttackRange { get; }
    protected abstract float AttackCooldown { get; }

    private void Update()
    {
        if (!HasValidTarget())
        {
            currentTarget = null;
            FindTarget();
        }

        if (currentTarget == null)
        {
            GoToDefaultTarget();
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            currentTarget.position
        );

        if (distance > AttackRange)
        {
            MoveTo(currentTarget.position);
            return;
        }

        StopMove();
        TryAttack();
    }
    private void TryAttack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        Attack();
        attackTimer = AttackCooldown;
    }

    protected abstract void Attack();

    protected void FindTarget()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(
            transform.position,
            DetectionRange
        );

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D obj in objects)
        {
            IDamageable damageable =
                obj.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            if (damageable.Team == team)
                continue;

            Component component = damageable as Component;

            if (component == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                component.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = component.transform;
            }
        }

        if (closestEnemy != null)
        {
            SetTarget(closestEnemy);

            Debug.Log(
                name + " нашёл цель: " + closestEnemy.name
            );
        }
    }
    private bool HasValidTarget()
    {
        if (currentTarget == null)
            return false;

        if (!currentTarget.gameObject.activeInHierarchy)
            return false;

        IDamageable damageable =
            currentTarget.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return false;

        return true;
    }
    private void GoToDefaultTarget()
    {
        if (defaultTarget != null)
        {
            MoveTo(defaultTarget.position);
        }
    }
}
