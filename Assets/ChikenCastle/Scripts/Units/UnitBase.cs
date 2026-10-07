using System;
using UnityEngine;
using UnityEngine.Events;

public enum Team
{
    LeftPlayer,
    RightPlayer
}

public class UnitBase : MonoBehaviour, IDamageable
{
    public Team Team => team;
    public bool IsDead => currentHealth <= 0; // или проверка флага isDead
    
    [SerializeField] protected UnitData unitData;
    [SerializeField] private UnityEvent dieEvent;
    [SerializeField] protected Team team;

    public event Action<UnitBase> OnDied;
    public UnitData UnitData => unitData;
    
    protected UnitAnimation unitAnimation;
    protected Transform defaultTarget;
    protected Transform currentTarget;
    private UnityEngine.AI.NavMeshAgent _agent;
    private  float currentHealth;


    protected virtual void Awake()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        unitAnimation = GetComponent<UnitAnimation>();

        // Настройки 2D для NavMeshPlus
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        if (unitData != null)
        {
            currentHealth = unitData.MaxHealth;
            _agent.speed = unitData.MoveSpeed;
        }
    }

    protected virtual void Start()
    {
        // Принудительно обнуляем Z, так как 2D-навигация работает строго на плоскости Z = 0
        Vector3 spawnPos2D = new Vector3(transform.position.x, transform.position.y, 0f);

        // Ищем ближайшую точку на сетке в радиусе 5 единиц
        if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos2D, out UnityEngine.AI.NavMeshHit hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
        {
            // Ставим юнит точно на найденную позицию сетки (с Z = 0)
            transform.position = hit.position;

            if (_agent != null)
            {
                _agent.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning($"{name} заспавнился слишком далеко от NavMesh! Координаты попытки: {transform.position}");
        }
    }

    protected void SetTarget(Transform target)
    {
        currentTarget = target;
    }
    
    public void SetDefaultTarget(Transform target)
    {
        defaultTarget = target;
    }

    public void SetTeam(Team newTeam)
    {
        team = newTeam;
    }
    protected void MoveTo(Vector3 position)
    {
        if (_agent == null)
            return;

        if (!_agent.isActiveAndEnabled)
            return;

        if (!_agent.isOnNavMesh)
            return;
 
        
        _agent.isStopped = false;
        _agent.SetDestination(position);
        unitAnimation?.Walk();
    }

    public  void StopMove()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector2.zero; // Гасим инерцию мгновенно
        }
    }

    public void ResumeMove()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
        }
    }

    protected virtual void LateUpdate()
    {
        // Жестко фиксируем Z на нуле, чтобы агент не смещал юнит по глубине
        Vector3 pos = transform.position;
        if (pos.z != 0f)
        {
            pos.z = 0f;
            transform.position = pos;
        }

        Flip();
    }

    private void Flip()
    {
        Transform target = currentTarget != null
            ? currentTarget
            : defaultTarget;

        if (target == null)
            return;

        float direction = target.position.x - transform.position.x;

        if (Mathf.Abs(direction) < 0.01f)
            return;

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);

        transform.localScale = scale;
    }
    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (_agent != null) _agent.enabled = false;
        unitAnimation?.Death();
        dieEvent?.Invoke(); 

        OnDied?.Invoke(this);
        //gameObject.SetActive(false); 
    }

}

