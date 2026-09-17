using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Связанные компоненты 🎯")]
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private PlayerResources playerResources;

    [Header("UI Компоненты 🖼️")]
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Данные Игрока 👤")]
    public PlayerData playerData;

    private readonly List<BuyUnitButton> _spawnedButtons = new List<BuyUnitButton>();
    private BuyUnitButton _selectedButton;

    private void OnEnable()
    {
        if (playerResources != null)
        {
            playerResources.OnGoldChanged += RefreshButtonsState;
        }
    }

    private void OnDisable()
    {
        if (playerResources != null)
        {
            playerResources.OnGoldChanged -= RefreshButtonsState;
        }
    }

    private void Start()
    {
        InitializeShop(playerData);
    }

    public void InitializeShop(PlayerData player)
    {
        ClearShop();

        if (player == null || player.SelectedUnits == null) return;

        foreach (UnitData unit in player.SelectedUnits)
        {
            if (unit == null) continue;

            GameObject newButton = Instantiate(buttonPrefab, buttonsParent);

            if (newButton.TryGetComponent<BuyUnitButton>(out var buyButton))
            {
                buyButton.Setup(unit, this);
                _spawnedButtons.Add(buyButton);
            }
        }

        // Первичное обновление состояния кнопок
        if (playerResources != null)
        {
            RefreshButtonsState(playerResources.CurrentGold);
        }
    }

    /// <summary>
    /// Вызывается автоматически при изменении баланса
    /// </summary>
    private void RefreshButtonsState(int currentGold)
    {
        foreach (var button in _spawnedButtons)
        {
            if (button != null)
            {
                button.UpdateInteractable(currentGold);
            }
        }
    }

    public void SelectButton(BuyUnitButton button, UnitData unitData)
    {
        if (_selectedButton == button)
        {
            CancelSelection();
            return;
        }

        if (playerResources != null && !playerResources.HasEnoughGold(unitData.Cost)) return;

        if (_selectedButton != null) _selectedButton.SetSelected(false);

        _selectedButton = button;
        _selectedButton.SetSelected(true);

        if (unitSpawner != null) unitSpawner.SetSelectedUnit(unitData, this);
    }

    public void OnUnitSpawned(UnitData unitData)
    {
        if (playerResources != null && unitData != null)
        {
            playerResources.TrySpendGold(unitData.Cost);
        }

        ResetSelection();
    }

    public void CancelSelection()
    {
        ResetSelection();
        if (unitSpawner != null) unitSpawner.ClearSelectedUnit();
    }

    public void ResetSelection()
    {
        if (_selectedButton != null)
        {
            _selectedButton.SetSelected(false);
            _selectedButton = null;
        }
    }

    private void ClearShop()
    {
        _spawnedButtons.Clear();
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }
    }
}