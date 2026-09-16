using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    [Header("Параметры атаки")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    public string EnemyTag { get; private set; } // 🏷️ Тег врага

    private Transform _defaultTarget; // Замок
    private Transform _currentTarget;  // Текущая цель (Замок или Враг)
    private IDamageable _targetDamageable;
    private IMovable _movable;

    private float _lastAttackTime;
    private bool _isAttacking;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();
    }

    public void SetEnemyTag(string tag)
    {
        EnemyTag = tag;
    }

    public void SetAttackTarget(Transform target)
    {
        _defaultTarget = target;
        SetCurrentTarget(target);
    }

    // 👁️ Вызывается из AggroArea при обнаружении врага
    public void OnEnemyDetected(Transform enemy)
    {
        // Переключаемся на врага, только если у нас сейчас нет другой приоритетной цели
        if (_currentTarget == _defaultTarget || _currentTarget == null)
        {
            SetCurrentTarget(enemy);
        }
    }

    private void SetCurrentTarget(Transform target)
    {
        _currentTarget = target;
        if (target != null)
        {
            _targetDamageable = target.GetComponent<IDamageable>();
            _movable?.SetTarget(target); // 🏃 Говорим движению переключиться на новую цель!
        }
    }

    private void Update()
    {
        // Если текущая цель была уничтожена — возвращаемся к замку 🏰
        if (_currentTarget == null)
        {
            if (_defaultTarget != null)
            {
                SetCurrentTarget(_defaultTarget);
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, _currentTarget.position);

        if (distance <= attackRange)
        {
            if (!_isAttacking)
            {
                _movable?.Stop();
                _isAttacking = true;
            }

            if (Time.time >= _lastAttackTime + attackCooldown)
            {
                _targetDamageable?.TakeDamage(damage);
                _lastAttackTime = Time.time;
            }
        }
        else if (_isAttacking)
        {
            _movable?.Resume();
            _isAttacking = false;
        }
    }
}