using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Script quản lý UI Game Over
/// Hiển thị khi player chết, cho phép quay lại Main Menu hoặc Restart
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;        // Panel chứa toàn bộ Game Over UI
    public TextMeshProUGUI gameOverText;    // Text "GAME OVER"
    public TextMeshProUGUI finalScoreText;  // Text hiển thị điểm cuối
    public Button returnButton;             // Nút quay về Main Menu
    public Button restartButton;            // Nút chơi lại (optional)

    [Header("Animation")]
    public float fadeInDuration = 1f;       // Thời gian fade in

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Ẩn Game Over panel khi bắt đầu
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Lấy CanvasGroup để làm hiệu ứng fade
        if (gameOverPanel != null)
        {
            canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }
        }

        // Gán sự kiện cho các button
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(OnReturnButtonClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        // Lắng nghe sự kiện Game Over từ GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver.AddListener(ShowGameOver);
        }
    }

    /// <summary>
    /// Hiển thị Game Over UI với hiệu ứng fade in
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel == null) return;

        gameOverPanel.SetActive(true);

        // Hiển thị điểm cuối
        if (finalScoreText != null && GameManager.Instance != null)
        {
            finalScoreText.text = "SCORE: " + GameManager.Instance.currentScore.ToString();
        }

        // Bắt đầu hiệu ứng fade in
        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled vì Time.timeScale có thể = 0
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Xử lý khi nhấn nút Return (quay về Main Menu)
    /// </summary>
    public void OnReturnButtonClicked()
    {
        Time.timeScale = 1f; // Reset time scale trước khi chuyển scene
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Xử lý khi nhấn nút Restart (chơi lại)
    /// </summary>
    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
