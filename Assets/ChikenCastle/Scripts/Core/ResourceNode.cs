using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Запас ресурса 💎")]
    [SerializeField] private int currentCapacity = 500;

    public bool IsEmpty => currentCapacity <= 0;

    /// <summary>
    /// Извлекает ресурс из шахты
    /// </summary>
    public int ExtractResource(int amount)
    {
        int extracted = Mathf.Min(amount, currentCapacity);
        currentCapacity -= extracted;

        if (IsEmpty)
        {
            Debug.Log($"⛏️ Шахта {gameObject.name} истощена!");
        }

        return extracted;
    }
}