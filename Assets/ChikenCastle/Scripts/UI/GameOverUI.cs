using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Замки Игроков 🏰")]
    [SerializeField] private CastleHealth leftCastle;
    [SerializeField] private CastleHealth rightCastle;

    [Header("Элементы UI 🖼️")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text winnerText;

    [Header("Цвета Игроков 🎨")]
    [SerializeField] private Color leftPlayerColor = Color.blue;
    [SerializeField] private Color rightPlayerColor = Color.red;

    private void Awake()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (leftCastle != null) leftCastle.OnCastleDestroyed += OnCastleDestroyed;
        if (rightCastle != null) rightCastle.OnCastleDestroyed += OnCastleDestroyed;
    }

    private void OnDisable()
    {
        if (leftCastle != null) leftCastle.OnCastleDestroyed -= OnCastleDestroyed;
        if (rightCastle != null) rightCastle.OnCastleDestroyed -= OnCastleDestroyed;
    }

    private void OnCastleDestroyed(CastleHealth destroyedCastle)
    {
        if (endPanel == null) return;

        endPanel.SetActive(true);

        // Определяем победителя
        if (destroyedCastle == leftCastle)
        {
            ShowWinner("Правый игрок", rightPlayerColor);
        }
        else if (destroyedCastle == rightCastle)
        {
            ShowWinner("Левый игрок", leftPlayerColor);
        }

        // Останавливаем время в игре
        Time.timeScale = 0f;
    }

    private void ShowWinner(string winnerName, Color textColor)
    {
        if (winnerText != null)
        {
            winnerText.text = $"ПОБЕДИЛ {winnerName.ToUpper()}!";
            winnerText.color = textColor;
        }
    }

    /// <summary>
    /// Метод для привязки к кнопке Перезапуска (Button OnClick)
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // Обязательно снимаем паузу!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}