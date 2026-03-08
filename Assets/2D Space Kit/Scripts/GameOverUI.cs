using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Script quản lý UI Game Over
/// Hiển thị khi BattleFlow bật gameOverPanel
/// Hiệu ứng fade in và các button Return/Restart
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI gameOverText;    // Text "GAME OVER"
    public TextMeshProUGUI finalScoreText;  // Text hiển thị điểm cuối
    public Button returnButton;             // Nút quay về Main Menu
    public Button restartButton;            // Nút chơi lại

    [Header("Animation")]
    public float fadeInDuration = 1f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Lấy CanvasGroup để làm hiệu ứng fade
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void OnEnable()
    {
        // Khi panel được bật, hiển thị điểm và fade in
        if (finalScoreText != null && GameManager.Instance != null)
        {
            finalScoreText.text = "SCORE: " + GameManager.Instance.currentScore.ToString();
        }

        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
