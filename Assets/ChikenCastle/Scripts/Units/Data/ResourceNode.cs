using UnityEngine;

public class ResourceNode : MonoBehaviour, IResourceSource
{
    [SerializeField] private int resourceAmount = 100;

    public bool HasResource => resourceAmount > 0;

    public int Gather(int amount)
    {
        if (!HasResource)
            return 0;

        int gatheredAmount = Mathf.Min(amount, resourceAmount);

        resourceAmount -= gatheredAmount;

        Debug.Log($"Собрано: {gatheredAmount}. Осталось: {resourceAmount}");

        return gatheredAmount;
    }
}