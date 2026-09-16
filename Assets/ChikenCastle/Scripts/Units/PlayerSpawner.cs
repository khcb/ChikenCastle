using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Префабы и цели")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform enemyCastle;

    [Header("Настройки команды")]
    [SerializeField] private string myTag = "LeftPlayer";      // Тег для спавнящихся юнитов
    [SerializeField] private string enemyTag = "RightPlayer";  // Тег врага

    [Header("Зона спавна")]
    [SerializeField] private Collider2D spawnArea;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // Проверяем клик мыши через новую Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0; // Для 2D фиксируем Z

            // Проверяем, попал ли клик в разрешенную зону спавна
            if (spawnArea != null && spawnArea.OverlapPoint(worldPosition))
            {
                SpawnAndSendUnit(worldPosition);
            }
        }
    }

    private void SpawnAndSendUnit(Vector3 position)
    {
        // 1. Создаем юнита
        GameObject newUnit = Instantiate(unitPrefab, position, Quaternion.identity);

        // 2. Назначаем юниту его тег (LeftPlayer или RightPlayer)
        newUnit.tag = myTag;

        // 3. Отправляем в движение к замку
        IMovable movable = newUnit.GetComponent<IMovable>();
        if (movable != null && enemyCastle != null)
        {
            movable.SetTarget(enemyCastle);
        }

        // 4. Передаем цель для атаки
        UnitAttack attack = newUnit.GetComponent<UnitAttack>();
        if (attack != null && enemyCastle != null)
        {
            attack.SetEnemyTag(enemyTag); // 🏷️ Сообщаем юниту, кто его враг
            attack.SetAttackTarget(enemyCastle);
        }
    }
}