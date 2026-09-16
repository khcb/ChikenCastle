using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour, IMovable
{
    private NavMeshAgent _agent;
    private Transform _target;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        // 💡 Настройки 2D для NavMeshPlus
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (_target != null)
        {
            _agent.SetDestination(_target.position);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

}