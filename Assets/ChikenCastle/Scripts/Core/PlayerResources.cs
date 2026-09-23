using System;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [Header("Ресурс")]
    [SerializeField] private int resources = 100;

    public int CurrentResources => resources;

    public event Action<int> OnResourcesChanged;


    public bool HasEnoughResources(int amount)
    {
        return resources >= amount;
    }

    public bool TrySpendResources(int amount)
    {
        if (amount <= 0)
            return false;

        if (resources < amount)
            return false;

        resources -= amount;

        OnResourcesChanged?.Invoke(resources);

        return true;
    }

    public void AddResources(int amount)
    {
        if (amount <= 0)
            return;

        resources += amount;

        Debug.Log(
            $"Получено ресурсов: {amount}. Всего: {resources}"
        );

        OnResourcesChanged?.Invoke(resources);
    }
}