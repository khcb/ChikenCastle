using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSpawner : MonoBehaviour
{
    [Header("Базы и Цели 🏰")]
    [SerializeField] private Transform myCastle;      // Своя база (для сдачи ресурсов сборщиками)
    [SerializeField] private Transform enemyCastle;   // Вражеский замок (для атаки бойцами)

    [Header("Настройки команды 🏷️")]
    [SerializeField] private string myTag = "LeftPlayer";      // Тег игрока
    [SerializeField] private string enemyTag = "RightPlayer";  // Тег врага

    [Header("Зона спавна и Кошелек 🪙")]
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private PlayerResources playerResources; // Нужен ТОЛЬКО для передачи в ResourceGatherer

    private Camera _mainCamera;
    private Vector3 _spawnPoint;

    private UnitData _selectedUnit;
    private ShopManager _ownerShopManager;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void SetSelectedUnit(UnitData unitData, ShopManager shopManager = null)
    {
        _selectedUnit = unitData;
        _ownerShopManager = shopManager;
    }

    public void ClearSelectedUnit()
    {
        _selectedUnit = null;
        _ownerShopManager = null;
    }

    private void Update()
    {
        if (_selectedUnit == null) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            float cameraDistance = Mathf.Abs(_mainCamera.transform.position.z);
            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cameraDistance));
            _spawnPoint = new Vector3(worldPoint.x, worldPoint.y, 0f);

            if (spawnArea != null && spawnArea.OverlapPoint(_spawnPoint))
            {
                SpawnAndSendUnit();
            }
        }
    }

    public void SpawnAndSendUnit()
    {
        if (_selectedUnit == null || _selectedUnit.UnitPrefab == null) return;

        // 1. Спавн юнита в выбранной точке
        GameObject newUnit = Instantiate(_selectedUnit.UnitPrefab, _spawnPoint, Quaternion.identity);
        newUnit.tag = myTag;

        // 2. Инициализация параметров юнита (Здоровье и Движение)
        if (newUnit.TryGetComponent<UnitHealth>(out var health))
        {
            health.Init(_selectedUnit);
        }

        if (newUnit.TryGetComponent<UnitMovement>(out var movement))
        {
            movement.Init(_selectedUnit);
        }

        // 3. Настройка специфичного поведения (Сборщик или Boец)
        if (_selectedUnit.Type == UnitType.Gatherer)
        {
            SetupGatherer(newUnit);
        }
        else
        {
            SetupCombatUnit(newUnit, movement);
        }

        // 4. Уведомляем ShopManager, что спавн успешен (он спишет золото и сбросит выбор)
        if (_ownerShopManager != null)
        {
            _ownerShopManager.OnUnitSpawned(_selectedUnit);
        }

        ClearSelectedUnit();
    }

    /// <summary>
    /// Настройка логики Сборщика ресурсов
    /// </summary>
    private void SetupGatherer(GameObject unitObj)
    {
        if (unitObj.TryGetComponent<ResourceGatherer>(out var gatherer))
        {
            Transform nearestNode = FindNearestResourceNode(unitObj.transform.position);

            if (nearestNode != null)
            {
                // Сборщику передается база и кошелек, куда складывать добычу
                gatherer.InitGatherer(_selectedUnit, nearestNode, myCastle, playerResources);
            }
            else
            {
                Debug.LogWarning("⚠️ UnitSpawner: На карте не найдено ни одной шахты с ресурсами!");
            }
        }
        else
        {
            Debug.LogError($"⚠️ На префабе {unitObj.name} не найден компонент ResourceGatherer!");
        }
    }

    /// <summary>
    /// Настройка логики Боевого юнита (Melee / Ranged)
    /// </summary>
    private void SetupCombatUnit(GameObject unitObj, UnitMovement movement)
    {
        if (movement != null && enemyCastle != null)
        {
            movement.SetTarget(enemyCastle);
        }

        if (unitObj.TryGetComponent<UnitAttack>(out var attack))
        {
            attack.SetEnemyTag(enemyTag);
            attack.SetAttackTarget(enemyCastle);
        }
    }

    /// <summary>
    /// Автоматический поиск ближайшей шахты на сцене
    /// </summary>
    private Transform FindNearestResourceNode(Vector3 spawnPos)
    {
        ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (var node in nodes)
        {
            if (node.IsEmpty) continue;

            float dist = Vector3.Distance(spawnPos, node.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = node.transform;
            }
        }

        return nearest;
    }
}