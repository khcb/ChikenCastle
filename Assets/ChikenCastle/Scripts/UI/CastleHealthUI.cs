using UnityEngine;
using TMPro;

public class CastleHealthUI : MonoBehaviour
{
    [Header("UI Ссылки 🖼️")]
    [SerializeField] private CastleHealth castleHealth;
    [SerializeField] private TMP_Text healthText;

    private void OnEnable()
    {
        if (castleHealth != null)
        {
            castleHealth.OnHealthChanged += UpdateHealthText;
        }
    }

    private void OnDisable()
    {
        if (castleHealth != null)
        {
            castleHealth.OnHealthChanged -= UpdateHealthText;
        }
    }

    // 🔴 Исправлено: заменено (int currentHealth, int maxHealth) на (float, float)
    private void UpdateHealthText(float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            // Округляем в большую сторону для красивого отображения (например, 99.2 HP покажет как 100)
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }
}