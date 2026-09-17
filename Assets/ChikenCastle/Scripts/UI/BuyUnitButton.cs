using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyUnitButton : MonoBehaviour
{
    [Header("UI Компоненты 🖼️")]
    [SerializeField] private Button buttonComponent;
    [SerializeField] private Image backgroundImage; // Фоновое изображение кнопки
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    [Header("Настройки Подсветки 🎨")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private GameObject selectionBorder; // Необязательно: объект рамки, если есть

    private UnitData _unitData;
    private ShopManager _shopManager;

    public void Setup(UnitData unitData, ShopManager shopManager)
    {
        _unitData = unitData;
        _shopManager = shopManager;

        if (priceText != null) priceText.text = unitData.Cost.ToString();
        if (nameText != null) nameText.text = unitData.UnitName;
        if (iconImage != null && unitData.UnitIcon != null) iconImage.sprite = unitData.UnitIcon;

        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnClick);
        }

        // По умолчанию снимаем выделение
        SetSelected(false);
    }

    /// <summary>
    /// Обновляет доступность кнопки по балансу
    /// </summary>
    public void UpdateInteractable(int currentGold)
    {
        if (_unitData == null || buttonComponent == null) return;

        bool hasMoney = currentGold >= _unitData.Cost;
        buttonComponent.interactable = hasMoney;
    }

    private void OnClick()
    {
        if (_unitData != null && _shopManager != null)
        {
            _shopManager.SelectButton(this, _unitData);
        }
    }

    /// <summary>
    /// Визуальная подсветка выбранной кнопки
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        // 1. Изменение цвета фона кнопки
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : normalColor;
        }

        // 2. Включение/выключение объекта рамки (если перетащен в Инспектор)
        if (selectionBorder != null)
        {
            selectionBorder.SetActive(isSelected);
        }
    }
}