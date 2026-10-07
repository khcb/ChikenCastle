using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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

    [Header("Настройки группы")]
    [SerializeField] private float spawnSpacing = 0.5f;

    [Header("Магазин")]
    [SerializeField] private ShopManager shopManager;

    private UnitData selectedUnit;
    private int selectedCount;

    // Сколько живых юнитов каждого типа сейчас находится на карте
    private Dictionary<UnitData, int> spawnedUnits = new();


    // =========================================================
    // ВЫБОР ЮНИТА
    // =========================================================

    public void SetSelectedUnit(
        UnitData unitData,
        int count)
    {
        selectedUnit = unitData;
        selectedCount = count;

        Debug.Log(
            $"SPAWNER: {unitData.UnitName} x{count}"
        );
    }


    public void ClearSelectedUnit()
    {
        selectedUnit = null;
        selectedCount = 0;

        Debug.Log("SPAWNER: выбор очищен");
    }


    // =========================================================
    // INPUT
    // =========================================================

private void Update()
{
    if (selectedUnit == null)
        return;

    // ПК
    if (Mouse.current != null &&
        Mouse.current.leftButton.wasPressedThisFrame)
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        TrySpawn(
            Mouse.current.position.ReadValue()
        );
    }

    // Телефон
    if (Touchscreen.current != null &&
        Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
    {
        int touchId =
            Touchscreen.current.primaryTouch.touchId.ReadValue();

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touchId))
        {
            return;
        }

        TrySpawn(
            Touchscreen.current.primaryTouch.position.ReadValue()
        );
    }
}


    // =========================================================
    // ПОПЫТКА СПАВНА
    // =========================================================

    private void TrySpawn(Vector2 screenPosition)
{
    // -----------------------------------------------------
    // ПРОВЕРЯЕМ ВЫБОР
    // -----------------------------------------------------

    if (selectedUnit == null)
    {
        Debug.LogWarning(
            "SPAWNER: юнит не выбран!"
        );

        return;
    }

    if (selectedCount <= 0)
    {
        Debug.LogWarning(
            "SPAWNER: количество юнитов <= 0!"
        );

        return;
    }


    // -----------------------------------------------------
    // ПРОВЕРЯЕМ ЗОНУ
    // -----------------------------------------------------

    if (spawnCollider == null)
    {
        Debug.LogError(
            "SPAWNER: Spawn Collider НЕ назначен!"
        );

        return;
    }

    if (Camera.main == null)
    {
        Debug.LogError(
            "SPAWNER: Camera.main не найдена!"
        );

        return;
    }


    Vector3 worldPosition =
        Camera.main.ScreenToWorldPoint(
            screenPosition
        );

    Debug.Log(
        $"SPAWNER: позиция = {worldPosition}"
    );


    if (!spawnCollider.OverlapPoint(worldPosition))
    {
        Debug.Log(
            "SPAWNER: клик ВНЕ зоны размещения"
        );

        return;
    }


    // -----------------------------------------------------
    // ПРОВЕРЯЕМ ЛИМИТ
    // -----------------------------------------------------

    if (!CanAddUnits(
            selectedUnit,
            selectedCount))
    {
        Debug.Log(
            $"SPAWNER: недостаточно свободных мест " +
            $"для {selectedUnit.UnitName}"
        );

        return;
    }


    // -----------------------------------------------------
    // СОЗДАЁМ ГРУППУ
    // -----------------------------------------------------

    Debug.Log(
        $"SPAWNER: размещаем " +
        $"{selectedUnit.UnitName} x{selectedCount}"
    );


    for (int i = 0; i < selectedCount; i++)
    {
        Vector3 spawnPosition =
            worldPosition +
            GetSpawnOffset(i);

        GameObject unitObject =
            Spawn(
                selectedUnit,
                spawnPosition
            );

        if (unitObject == null)
        {
            Debug.LogError(
                "SPAWNER: не удалось создать юнита!"
            );

            return;
        }
    }


    // -----------------------------------------------------
    // УСПЕШНО СОЗДАЛИ
    // -----------------------------------------------------

    Debug.Log(
        $"SPAWNER: успешно создано " +
        $"{selectedCount} юнитов"
    );


    // -----------------------------------------------------
    // СПИСЫВАЕМ РЕСУРСЫ
    // -----------------------------------------------------

    if (shopManager != null)
    {
        Debug.Log(
            $"SPAWNER: вызываю OnUnitsSpawned: " +
            $"{selectedUnit.UnitName} x{selectedCount}"
        );

        shopManager.OnUnitsSpawned(
            selectedUnit,
            selectedCount
        );
    }
    else
    {
        Debug.LogError(
            "SPAWNER: ShopManager НЕ назначен!"
        );
    }


    // -----------------------------------------------------
    // ЗВУК
    // -----------------------------------------------------

    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlaySound(
            SoundType.Spawn
        );
    }
}


    // =========================================================
    // СОЗДАНИЕ ОДНОГО ЮНИТА
    // =========================================================

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
                $"SPAWNER: у {data.UnitName} " +
                "не назначен UnitPrefab!"
            );

            return null;
        }


        // -----------------------------------------------------
        // СОЗДАЁМ
        // -----------------------------------------------------

        GameObject unitObject =
            Instantiate(
                data.UnitPrefab,
                position,
                Quaternion.identity
            );


        // -----------------------------------------------------
        // ПОЛУЧАЕМ UNIT BASE
        // -----------------------------------------------------

        UnitBase unit =
            unitObject.GetComponent<UnitBase>();

        if (unit == null)
        {
            Debug.LogError(
                $"SPAWNER: на префабе {data.UnitName} " +
                "нет UnitBase!"
            );

            Destroy(unitObject);

            return null;
        }


        // -----------------------------------------------------
        // СЧИТАЕМ ЮНИТА
        // -----------------------------------------------------

        AddSpawnedUnit(data);


        // Когда юнит умрёт — уменьшим счётчик
        unit.OnDied += OnUnitDied;


        // -----------------------------------------------------
        // ОБЩИЕ НАСТРОЙКИ
        // -----------------------------------------------------

        unit.SetTeam(team);

        unit.SetDefaultTarget(enemyCastle);


        // -----------------------------------------------------
        // ЕСЛИ ЭТО СБОРЩИК
        // -----------------------------------------------------

        GathererUnit gatherer =
            unitObject.GetComponent<GathererUnit>();

        if (gatherer != null)
        {
            gatherer.SetPlayerResources(
                playerResources
            );

            gatherer.SetHomeBase(
                homeCastle
            );
        }


        return unitObject;
    }


    // =========================================================
    // ЛИМИТЫ ЮНИТОВ
    // =========================================================

    public int GetUnitCount(UnitData data)
    {
        if (data == null)
            return 0;

        if (spawnedUnits.TryGetValue(
                data,
                out int count))
        {
            return count;
        }

        return 0;
    }


    public bool CanAddUnit(UnitData data)
    {
        if (data == null)
            return false;

        int currentCount =
            GetUnitCount(data);

        return currentCount < data.MaxUnits;
    }


    public bool CanAddUnits(
        UnitData data,
        int amount)
    {
        if (data == null)
            return false;

        if (amount <= 0)
            return false;

        int currentCount =
            GetUnitCount(data);

        return currentCount + amount <=
               data.MaxUnits;
    }


    private void AddSpawnedUnit(
        UnitData data)
    {
        if (!spawnedUnits.ContainsKey(data))
        {
            spawnedUnits[data] = 0;
        }

        spawnedUnits[data]++;
    }


    // =========================================================
    // ЮНИТ УМЕР
    // =========================================================

    private void OnUnitDied(UnitBase unit)
    {
        if (unit == null)
            return;

        UnitData data =
            unit.UnitData;

        if (data == null)
            return;

        if (!spawnedUnits.ContainsKey(data))
            return;


        spawnedUnits[data]--;


        if (spawnedUnits[data] <= 0)
        {
            spawnedUnits.Remove(data);
        }


        Debug.Log(
            $"SPAWNER: {data.UnitName} " +
            $"{GetUnitCount(data)}/{data.MaxUnits}"
        );


        // Обновляем кнопки магазина
        UpdateShopButtons();
    }


    // =========================================================
    // ОБНОВЛЕНИЕ КНОПОК
    // =========================================================

    private void UpdateShopButtons()
    {
        if (shopManager == null)
            return;

        shopManager.UpdateUnitButtons();
    }


    // =========================================================
    // РАСКЛАДКА ГРУППЫ
    // =========================================================

    private Vector3 GetSpawnOffset(
        int index)
    {
        int columns = 3;

        int row =
            index / columns;

        int column =
            index % columns;

        float x =
            (column - 1) *
            spawnSpacing;

        float y =
            row *
            spawnSpacing;

        return new Vector3(
            x,
            y,
            0f
        );
    }
}