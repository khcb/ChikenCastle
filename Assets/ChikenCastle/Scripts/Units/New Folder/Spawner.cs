using UnityEngine;
using UnityEngine.InputSystem;

public class Spawner : MonoBehaviour
{
    [Header("Игрок")]
    [SerializeField] private Team team;
    [SerializeField] private PlayerResources playerResources;

    [Header("Цель")]
    [SerializeField] private Transform enemyCastle;
    [Header("Свой замок")]
    [SerializeField] private Transform homeCastle;

    [Header("Зона размещения")]
    [SerializeField] private Collider2D spawnCollider;

    private UnitData selectedUnit;
    private ShopManager shopManager;

    public void SetSelectedUnit(UnitData unitData, ShopManager shop)
{
    selectedUnit = unitData;
    shopManager = shop;

    Debug.Log("SPAWNER: получил юнита: " + unitData.UnitName);
}

    public void ClearSelectedUnit()
    {
        selectedUnit = null;
        shopManager = null;

        Debug.Log("SPAWNER: выбор очищен");
    }

    private void Update()
    {
        // Если юнит не выбран — ничего не делаем
        if (selectedUnit == null)
            return;

        // Проверяем нажатие мыши
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("SPAWNER: получен клик мыши");

            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        // Проверяем Collider
        if (spawnCollider == null)
        {
            Debug.LogError(
                "SPAWNER: Spawn Collider НЕ назначен!"
            );

            return;
        }

        // Проверяем камеру
        if (Camera.main == null)
        {
            Debug.LogError(
                "SPAWNER: Camera.main не найдена!"
            );

            return;
        }

        // Получаем позицию мыши в мире
        Vector2 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

        Debug.Log(
            "SPAWNER: позиция мыши = " + mousePosition
        );

        // Проверяем попадание в нашу зону
        bool canPlace =
            spawnCollider.OverlapPoint(mousePosition);

        if (!canPlace)
        {
            Debug.Log(
                "SPAWNER: клик ВНЕ зоны размещения"
            );

            return;
        }

        Debug.Log(
            "SPAWNER: клик ВНУТРИ зоны размещения"
        );

        // Создаём юнита
        GameObject unitObject = Spawn(
            selectedUnit,
            mousePosition
        );

        // Если создание не удалось
        if (unitObject == null)
        {
            Debug.LogError(
                "SPAWNER: юнит НЕ был создан!"
            );

            return;
        }

        Debug.Log(
            "SPAWNER: юнит успешно создан"
        );

        // Сообщаем магазину
        if (shopManager != null)
        {
            shopManager.OnUnitSpawned(selectedUnit);
        }

        // Сбрасываем выбор
        ClearSelectedUnit();
    }

    private GameObject Spawn(
        UnitData data,
        Vector3 position)
    {
        if (data == null)
        {
            Debug.LogError(
                "SPAWNER: UnitData = NULL"
            );

            return null;
        }

        if (data.UnitPrefab == null)
        {
            Debug.LogError(
                "SPAWNER: у " +
                data.UnitName +
                " не назначен UnitPrefab!"
            );

            return null;
        }

        GameObject unitObject = Instantiate(
            data.UnitPrefab,
            position,
            Quaternion.identity
        );

        UnitBase unit =
            unitObject.GetComponent<UnitBase>();

        if (unit == null)
        {
            Debug.LogError(
                "SPAWNER: на префабе " +
                data.UnitName +
                " нет UnitBase!"
            );

            Destroy(unitObject);

            return null;
        }

        // Назначаем команду
        unit.SetTeam(team);
        // Назначаем замок 
        unit.SetDefaultTarget(enemyCastle);

        GathererUnit gatherer = unitObject.GetComponent<GathererUnit>();

        if (gatherer != null)
        {
            gatherer.SetPlayerResources(playerResources);
            gatherer.SetHomeBase(homeCastle);
        }

        return unitObject;
    }
}

