using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ResourceGatherer : MonoBehaviour
{
    private UnitData _unitData;
    private Transform _targetNode;
    private Transform _homeBase;
    private PlayerResources _playerResources;

    private IMovable _movable;
    private NavMeshAgent _agent;
    private int _carriedAmount = 0;
    private bool _isGathering = false;

    private void Awake()
    {
        _movable = GetComponent<IMovable>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public void InitGatherer(UnitData data, Transform node, Transform baseTransform, PlayerResources resources)
    {
        _unitData = data;
        _targetNode = node;
        _homeBase = baseTransform;
        _playerResources = resources;

        if (_homeBase == null)
        {
            Debug.LogError("❌ [Gatherer Error] _homeBase (Замок) не передан в InitGatherer! Проверьте UnitSpawner в Инспекторе.");
        }

        if (_movable != null && _targetNode != null)
        {
            _movable.SetTarget(_targetNode);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Пришел на шахту за ресурсами
        if (_targetNode != null && other.transform == _targetNode && _carriedAmount == 0 && !_isGathering)
        {
            if (other.TryGetComponent<ResourceNode>(out var node))
            {
                StartCoroutine(GatherRoutine(node));
            }
        }
        // 2. Пришел к замку с ресурсами
        else if (_carriedAmount > 0 && (_homeBase != null && other.transform == _homeBase || other.GetComponent<CastleHealth>() != null))
        {
            DeliverResources();
        }
    }

    private IEnumerator GatherRoutine(ResourceNode node)
    {
        _isGathering = true;

        // Останавливаем движение
        if (_agent != null) _agent.isStopped = true;

        float gatherTime = _unitData != null ? _unitData.GatherSpeed : 1.5f;
        yield return new WaitForSeconds(gatherTime);

        if (node != null)
        {
            int capacity = _unitData != null ? _unitData.GatherCapacity : 25;
            _carriedAmount = node.ExtractResource(capacity);
            Debug.Log($"⛏️ Добыто ресурсов: {_carriedAmount}");
        }

        _isGathering = false;

        if (_carriedAmount <= 0)
        {
            Debug.LogWarning("⚠️ Шахта пуста, сборщик удален.");
            Destroy(gameObject);
            yield break;
        }

        // 🚀 ОТПРАВКА К ЗАМКУ
        if (_homeBase != null)
        {
            Debug.Log($"🏃‍♂️ Отправляем сборщика к замку: {_homeBase.name} (Позиция: {_homeBase.position})");

            // Принудительно снятие паузы с NavMeshAgent
            if (_agent != null)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_homeBase.position);
            }
            
            if (_movable != null)
            {
                _movable.Resume();
                _movable.SetTarget(_homeBase);
            }
        }
        else
        {
            Debug.LogError("❌ ОШИБКА: У сборщика НЕТ ссылки на замок (_homeBase == null)! Он не знает, куда нести золото.");
        }
    }

    private void DeliverResources()
    {
        if (_playerResources != null && _carriedAmount > 0)
        {
            _playerResources.AddResource(_carriedAmount);
            Debug.Log($"💰 Сборщик успешно сдал {_carriedAmount} золота!");
        }

        Destroy(gameObject);
    }
}