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

    private BuyUnitButton selectedButton;

    private void OnEnable()
    {
        if (playerResources != null)
        {
            playerResources.OnResourcesChanged += RefreshButtons;
        }
    }

    private void OnDisable()
    {
        if (playerResources != null)
        {
            playerResources.OnResourcesChanged -= RefreshButtons;
        }
    }

    private void Start()
    {
        InitializeShop();
    }

    // Создаём кнопки выбранных игроком юнитов
    private void InitializeShop()
    {
        ClearShop();

        if (playerData == null)
        {
            Debug.LogWarning("ShopManager: PlayerData не задан.");
            return;
        }

        if (playerData.SelectedUnits == null)
        {
            Debug.LogWarning("ShopManager: у PlayerData нет выбранных юнитов.");
            return;
        }

        foreach (UnitData unitData in playerData.SelectedUnits)
        {
            if (unitData == null)
                continue;

            GameObject buttonObject =
                Instantiate(buttonPrefab, buttonsParent);

            if (!buttonObject.TryGetComponent<BuyUnitButton>(
                    out BuyUnitButton button))
            {
                Debug.LogWarning(
                    "На buttonPrefab отсутствует BuyUnitButton."
                );

                continue;
            }

            button.Setup(unitData, this);
            spawnedButtons.Add(button);
        }

        if (playerResources != null)
        {
            RefreshButtons(playerResources.CurrentResources);
        }
    }

    // Обновляем доступность кнопок после изменения денег
    private void RefreshButtons(int currentGold)
    {
        foreach (BuyUnitButton button in spawnedButtons)
        {
            if (button == null)
                continue;

            button.UpdateInteractable(currentGold);
        }
    }

    // Вызывается BuyUnitButton при нажатии
    public void SelectButton(
        BuyUnitButton button,
        UnitData unitData)
    {
        if (button == null || unitData == null)
            return;

        // Повторное нажатие отменяет выбор
        if (selectedButton == button)
        {
            CancelSelection();
            return;
        }

        // Проверяем деньги
        if (playerResources != null &&
            !playerResources.HasEnoughResources(unitData.Cost))
        {
            return;
        }

        // Снимаем выделение с предыдущей кнопки
        if (selectedButton != null)
        {
            selectedButton.SetSelected(false);
        }

        // Запоминаем новую кнопку
        selectedButton = button;
        selectedButton.SetSelected(true);

        if (spawner == null)
{
    Debug.LogError("SHOP: Spawner НЕ назначен!");
    return;
}

Debug.Log("SHOP: передаю юнита в Spawner: " + unitData.UnitName);

spawner.SetSelectedUnit(unitData, this);
    }

    // Вызывается Spawner после успешного создания юнита
    public void OnUnitSpawned(UnitData unitData)
    {
        if (unitData == null)
            return;

        // Списываем деньги только после успешного размещения
        if (playerResources != null)
        {
            playerResources.TrySpendResources(unitData.Cost);
        }

        CancelSelection();
    }

    // Отмена выбора
    public void CancelSelection()
    {
        if (selectedButton != null)
        {
            selectedButton.SetSelected(false);
            selectedButton = null;
        }

        if (spawner != null)
        {
            spawner.ClearSelectedUnit();
        }
    }

    // Удаляем старые кнопки
    private void ClearShop()
    {
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }

        spawnedButtons.Clear();
        selectedButton = null;
    }
}
