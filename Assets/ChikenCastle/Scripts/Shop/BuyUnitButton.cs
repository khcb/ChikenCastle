using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyUnitButton : MonoBehaviour
{
    [Header("UI Компоненты")]
    [SerializeField] private Button buttonComponent;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText; // Отображает цену, а при кулдауне — таймер

    [Header("Настройки подсветки")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private GameObject selectionBorder;

    private UnitData unitData;
    private ShopManager shopManager;
    private float cooldownTimer;

    public void Setup(UnitData unitData, ShopManager shopManager)
    {
        this.unitData = unitData;
        this.shopManager = shopManager;

        if (iconImage != null && unitData.UnitIcon != null)
            iconImage.sprite = unitData.UnitIcon;

        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnClick);
        }

        SetSelected(false);
        cooldownTimer = 0f;
        UpdateCooldownUI();
    }

    private void Update()
    {
        if (cooldownTimer <= 0f)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0f)
            cooldownTimer = 0f;

        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        bool cooldownFinished = cooldownTimer <= 0f;

        if (buttonComponent != null)
        {
            buttonComponent.interactable = cooldownFinished;
        }

        if (priceText != null && unitData != null)
        {
            if (cooldownFinished)
            {
                // Если кулдаун закончился, возвращаем текст стоимости
                priceText.text = unitData.Cost.ToString();
            }
            else
            {
                // Если идет кулдаун, показываем округленное время таймера
                priceText.text = Mathf.CeilToInt(cooldownTimer).ToString();
            }
        }
    }

    public void StartCooldown()
    {
        if (unitData == null)
            return;

        cooldownTimer = unitData.SpawnCooldown;
        UpdateCooldownUI();
    }

    private void OnClick()
    {
        if (unitData != null && shopManager != null)
        {
            shopManager.SelectButton(this, unitData);
        }
    }

    public void UpdateInteractable(int currentResources)
    {
        if (unitData == null || buttonComponent == null)
            return;

        bool hasResources = currentResources >= unitData.Cost;
        bool cooldownFinished = cooldownTimer <= 0f;

        buttonComponent.interactable = hasResources && cooldownFinished;
    }

    public void SetSelected(bool isSelected)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : normalColor;
        }

        if (selectionBorder != null)
        {
            selectionBorder.SetActive(isSelected);
        }
    }
}
