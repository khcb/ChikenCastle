using UnityEngine;
using UnityEngine.InputSystem; // 💡 Импортируем новую систему ввода

public class PlayerSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private GameObject unitPrefab;   // Префаб юнита
    [SerializeField] private Transform enemyCastle;   // Цель (Замок врага)
    [SerializeField] private Collider2D spawnArea;    // 2D-коллайдер зоны спавна

    private Camera _mainCamera;

    private void Awake()
    {
        // Кэшируем ссылку на главную камеру для оптимизации
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // 1. Проверяем, подключена ли мышь и нажата ли левая кнопка в этом кадре
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TrySpawnUnit();
        }
    }

    private void TrySpawnUnit()
    {
        // 2. Считываем позицию мыши на экране
        Vector2 screenPosition = Mouse.current.position.ReadValue();

        // 3. Переводим экранные пиксели в мировые 2D-координаты
        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
        mouseWorldPosition.z = 0f; // Фиксируем Z-координату для XY-плоскости

        // 4. Проверяем, попал ли клик в разрешенную 2D-зону
        if (spawnArea != null && spawnArea.OverlapPoint(mouseWorldPosition))
        {
            // 💡 В будущем здесь будет проверка: if (HasEnoughResources())
            SpawnAndSendUnit(mouseWorldPosition);
        }
    }

    private void SpawnAndSendUnit(Vector3 position)
    {
        // Создаем юнита в точке клика
        GameObject newUnit = Instantiate(unitPrefab, position, Quaternion.identity);

        // Передаем цель через интерфейс IMovable (принцип D в SOLID)
        IMovable movable = newUnit.GetComponent<IMovable>();
        if (movable != null && enemyCastle != null)
        {
            movable.SetTarget(enemyCastle);
        }
    }
}