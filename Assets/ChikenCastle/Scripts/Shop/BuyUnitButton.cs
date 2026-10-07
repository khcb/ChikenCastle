using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyUnitButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button buttonComponent;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text countText;

    [Header("Цвет")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    private UnitData unitData;
    private ShopManager shopManager;

    public UnitData UnitData => unitData;

    public void Setup(UnitData data, ShopManager manager)
    {
        unitData = data;
        shopManager = manager;

        priceText.text = data.Cost.ToString();

        if (iconImage != null)
            iconImage.sprite = data.UnitIcon;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnClick);

        SetCount(0);
        SetSelected(false);
    }

    private void OnClick()
    {
        if (unitData == null || shopManager == null)
            return;

        shopManager.AddUnit(unitData, this);
    }

    public void SetCount(int count)
    {
        if (countText == null)
            return;

        countText.text = count > 0
            ? $"×{count}"
            : "";
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage == null)
            return;

        backgroundImage.color =
            selected ? selectedColor : normalColor;
    }

    public void UpdateInteractable(
    int currentGold,
    bool canSpawn)
    {
        if (unitData == null)
            return;

        buttonComponent.interactable =
            currentGold >= unitData.Cost &&
            canSpawn;
    }
}