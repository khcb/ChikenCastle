using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Связанные компоненты")]
    [SerializeField] private Spawner spawner;
    [SerializeField] private PlayerResources playerResources;

    [Header("UI")]
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Данные игрока")]
    [SerializeField] private PlayerData playerData;

    private readonly List<BuyUnitButton> spawnedButtons = new();

    private UnitData selectedUnit;
    private int selectedCount;
    private BuyUnitButton selectedButton;


    // =========================================================
    // ЖИЗНЕННЫЙ ЦИКЛ
    // =========================================================

    private void OnEnable()
    {
        if (playerResources != null)
        {
            playerResources.OnResourcesChanged += OnResourcesChanged;
        }
    }

    private void OnDisable()
    {
        if (playerResources != null)
        {
            playerResources.OnResourcesChanged -= OnResourcesChanged;
        }
    }

    private void Start()
    {
        InitializeShop();
    }


    // =========================================================
    // РЕСУРСЫ И UI
    // =========================================================

    private void OnResourcesChanged(int currentResources)
    {
        UpdateUnitButtons();
    }

    public void UpdateUnitButtons()
    {
        if (playerResources == null)
            return;

        int currentResources =
            playerResources.CurrentResources;

        foreach (BuyUnitButton button in spawnedButtons)
        {
            if (button == null)
                continue;

            UnitData unitData =
                button.UnitData;

            if (unitData == null)
                continue;


            // Сколько уже выбрано
            int countToAdd = 1;

            if (selectedUnit == unitData)
            {
                countToAdd =
                    selectedCount + 1;
            }


            bool enoughResources =
                currentResources >=
                unitData.Cost * countToAdd;


            bool canAdd =
                spawner != null &&
                spawner.CanAddUnits(
                    unitData,
                    countToAdd
                );


            button.UpdateInteractable(
                currentResources,
                enoughResources && canAdd
            );
        }
    }


    // =========================================================
    // СОЗДАНИЕ МАГАЗИНА
    // =========================================================

    private void InitializeShop()
    {
        ClearShop();

        if (playerData == null)
        {
            Debug.LogWarning(
                "ShopManager: PlayerData не задан."
            );

            return;
        }

        if (playerData.SelectedUnits == null)
        {
            Debug.LogWarning(
                "ShopManager: у PlayerData нет выбранных юнитов."
            );

            return;
        }

        foreach (UnitData unitData in playerData.SelectedUnits)
        {
            if (unitData == null)
                continue;

            GameObject buttonObject =
                Instantiate(
                    buttonPrefab,
                    buttonsParent
                );

            if (!buttonObject.TryGetComponent<BuyUnitButton>(
                out BuyUnitButton button))
            {
                Debug.LogWarning(
                    "На buttonPrefab отсутствует BuyUnitButton."
                );

                Destroy(buttonObject);
                continue;
            }

            button.Setup(
                unitData,
                this
            );

            spawnedButtons.Add(button);
        }

        UpdateUnitButtons();
    }


    // =========================================================
    // ВЫБОР ЮНИТА
    // =========================================================

    public void AddUnit(
        UnitData unitData,
        BuyUnitButton button)
    {
        if (unitData == null || button == null)
            return;

        if (spawner == null)
        {
            Debug.LogError(
                "ShopManager: Spawner не назначен!"
            );

            return;
        }

        if (playerResources == null)
        {
            Debug.LogError(
                "ShopManager: PlayerResources не назначен!"
            );

            return;
        }


        // -----------------------------------------------------
        // Если нажали другой тип юнита
        // -----------------------------------------------------

        if (selectedUnit != null &&
            selectedUnit != unitData)
        {
            CancelSelection();
        }


        // -----------------------------------------------------
        // Сколько хотим выбрать
        // -----------------------------------------------------

        int newCount =
            selectedCount + 1;


        // -----------------------------------------------------
        // Проверяем лимит
        // -----------------------------------------------------

        if (!spawner.CanAddUnits(
                unitData,
                newCount))
        {
            Debug.Log(
                $"Лимит {unitData.UnitName} достигнут."
            );

            return;
        }


        // -----------------------------------------------------
        // Проверяем деньги
        // -----------------------------------------------------

        int totalCost =
            unitData.Cost * newCount;

        if (!playerResources.HasEnoughResources(
                totalCost))
        {
            Debug.Log(
                $"Недостаточно ресурсов для {unitData.UnitName} x{newCount}"
            );

            return;
        }


        // -----------------------------------------------------
        // Выбираем кнопку
        // -----------------------------------------------------

        if (selectedButton != button)
        {
            if (selectedButton != null)
            {
                selectedButton.SetSelected(false);
                selectedButton.SetCount(0);
            }

            selectedButton = button;
            selectedUnit = unitData;

            selectedButton.SetSelected(true);

            selectedCount = 0;
        }


        // -----------------------------------------------------
        // Увеличиваем количество
        // -----------------------------------------------------

        selectedCount++;

        selectedButton.SetCount(
            selectedCount
        );


        // -----------------------------------------------------
        // Передаём выбор Spawner
        // -----------------------------------------------------

        spawner.SetSelectedUnit(
            selectedUnit,
            selectedCount
        );


        Debug.Log(
            $"Выбрано: " +
            $"{selectedUnit.UnitName} x{selectedCount}"
        );


        UpdateUnitButtons();
    }


    // =========================================================
    // УСПЕШНЫЙ СПАВН
    // =========================================================

public void OnUnitsSpawned(
    UnitData unitData,
    int count)
{
    Debug.Log(
        $"SHOP: OnUnitsSpawned вызван! " +
        $"{unitData?.UnitName} x{count}"
    );

    if (unitData == null || count <= 0)
        return;

    if (playerResources == null)
    {
        Debug.LogError("SHOP: PlayerResources = NULL!");
        return;
    }

    int totalCost =
        unitData.Cost * count;

    Debug.Log(
        $"SHOP: пытаемся списать {totalCost}. " +
        $"Было ресурсов: {playerResources.CurrentResources}"
    );

    if (!playerResources.TrySpendResources(totalCost))
    {
        Debug.LogWarning(
            "SHOP: TrySpendResources вернул FALSE!"
        );

        return;
    }

    Debug.Log(
        $"SHOP: ресурсы списаны. " +
        $"Осталось: {playerResources.CurrentResources}"
    );

    CancelSelection();
}


    // =========================================================
    // ОТМЕНА ВЫБОРА
    // =========================================================

    public void CancelSelection()
    {
        if (selectedButton != null)
        {
            selectedButton.SetSelected(false);
            selectedButton.SetCount(0);
        }

        selectedButton = null;
        selectedUnit = null;
        selectedCount = 0;


        if (spawner != null)
        {
            spawner.ClearSelectedUnit();
        }


        UpdateUnitButtons();
    }


    // =========================================================
    // ОЧИСТКА
    // =========================================================

    private void ClearShop()
    {
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }

        spawnedButtons.Clear();

        selectedButton = null;
        selectedUnit = null;
        selectedCount = 0;
    }
}