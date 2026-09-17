using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour, IMovable
{
    [Header("Данные юнита (Scriptable Object) 📄")]
    [SerializeField] private UnitData unitData;

    private NavMeshAgent _agent;
    private Transform _target;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        // 💡 Настройки 2D для NavMeshPlus
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        // Если UnitData уже залинкован в префабе
        if (unitData != null)
        {
            ApplyData();
        }
    }

    /// <summary>
    /// Инициализация движения из ScriptableObject (вызывается при спавне)
    /// </summary>
    public void Init(UnitData data)
    {
        unitData = data;
        ApplyData();
    }

    private void ApplyData()
    {
        if (_agent != null && unitData != null)
        {
            _agent.speed = unitData.MoveSpeed; 
        }
    }

    private void Update()
    {
        if (_target != null && _agent.isOnNavMesh)
        {
            _agent.SetDestination(_target.position);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void Stop()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
        }
    }

    public void Resume()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
        }
    }
}