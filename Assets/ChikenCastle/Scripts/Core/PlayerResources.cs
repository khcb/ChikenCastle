using System;
using UnityEngine;
using TMPro;

public class PlayerResources : MonoBehaviour
{
    [Header("Настройки 🪙")]
    [SerializeField] private int currentGold = 100;
    [SerializeField] private TMP_Text goldText;

    // Событие передает текущее количество золота
    public event Action<int> OnGoldChanged;

    public int CurrentGold => currentGold;

    private void Start()
    {
        UpdateUI();
    }

    public bool HasEnoughGold(int amount) => currentGold >= amount;

    public void AddResource(int amount)
    {
        currentGold += amount;
        UpdateUI();
    }

    public bool TrySpendGold(int amount)
    {
        if (HasEnoughGold(amount))
        {
            currentGold -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (goldText != null) goldText.text = currentGold.ToString();
        
        // Уведомляем все подписанные магазины и UI о смене баланса
        OnGoldChanged?.Invoke(currentGold);
    }
}