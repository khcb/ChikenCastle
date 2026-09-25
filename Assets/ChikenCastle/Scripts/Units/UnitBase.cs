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
    
    [SerializeField] protected UnitData unitData;
    [SerializeField] private UnityEvent dieEvent;
    [SerializeField] protected Team team;
    
    
    
    protected Transform defaultTarget;
    protected Transform currentTarget;
    private UnityEngine.AI.NavMeshAgent _agent;
    private  float currentHealth;


    protected virtual void Awake()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Настройки 2D для NavMeshPlus
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        if (unitData != null)
        {
            currentHealth = unitData.MaxHealth;
            _agent.speed = unitData.MoveSpeed;
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
        dieEvent?.Invoke(); 
        gameObject.SetActive(false); 
    }
}

