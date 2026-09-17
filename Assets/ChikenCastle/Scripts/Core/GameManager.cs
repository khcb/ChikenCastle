using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Замки игроков 🏰")]
    [SerializeField] private CastleHealth leftPlayerCastle;
    [SerializeField] private CastleHealth rightPlayerCastle;

    [Header("UI Экраны 🖼️")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private void OnEnable()
    {
        if (leftPlayerCastle != null) leftPlayerCastle.OnCastleDestroyed += HandleCastleDestroyed;
        if (rightPlayerCastle != null) rightPlayerCastle.OnCastleDestroyed += HandleCastleDestroyed;
    }

    private void OnDisable()
    {
        if (leftPlayerCastle != null) leftPlayerCastle.OnCastleDestroyed -= HandleCastleDestroyed;
        if (rightPlayerCastle != null) rightPlayerCastle.OnCastleDestroyed -= HandleCastleDestroyed;
    }

    private void HandleCastleDestroyed(CastleHealth destroyedCastle)
    {
        // Пауза игры
        Time.timeScale = 0f;

        if (destroyedCastle == rightPlayerCastle)
        {
            // Пал замок врага — ПОБЕДА
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }
        else if (destroyedCastle == leftPlayerCastle)
        {
            // Пал наш замок — ПОРАЖЕНИЕ
            if (defeatPanel != null) defeatPanel.SetActive(true);
        }
    }

    // Метод для кнопки "Перезапуск"
    public void RestartGame()
    {
        Time.timeScale = 1f; // Снимаем паузу!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}