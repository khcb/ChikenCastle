using System;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [Header("Ресурс")]
    [SerializeField] private int resources = 100;

    [Header("Автоматическое получение")]
    [SerializeField] private int incomeAmount = 1;
    [SerializeField] private float incomeInterval = 3f;

    public int CurrentResources => resources;

    public event Action<int> OnResourcesChanged;

    private float incomeTimer;


    private void Update()
    {
        incomeTimer += Time.deltaTime;

        if (incomeTimer >= incomeInterval)
        {
            incomeTimer = 0f;

            AddResources(incomeAmount);
        }
    }


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

        Debug.Log(
            $"RESOURCES: потрачено {amount}. " +
            $"Осталось: {resources}"
        );

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