using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Điều khiển luồng game chính trong scene Battle:
/// - Game Over khi player chết
/// - Game Win khi tất cả enemy bị tiêu diệt
/// - Quay về Main Menu
/// </summary>
public class BattleFlow : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverUI;    // Panel Game Over
    public GameObject gameWinUI;     // Panel Game Win

    [Header("References")]
    public Health playerHealth;      // Health component của Player
    public GameObject bgMusic;       // Background music object

    private bool isGameEnded = false; // Tránh trigger nhiều lần

    private void Start()
    {
        // Ẩn cả 2 panel khi bắt đầu
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (gameWinUI != null) gameWinUI.SetActive(false);

        // Lắng nghe event khi player chết
        if (playerHealth != null)
        {
            playerHealth.onDead += OnGameOver;
        }

        // Reset bộ đếm enemy khi bắt đầu scene mới
        EnemyHealth.LivingEnemyCount = 0;
    }

    private void Update()
    {
        // Kiểm tra đã thắng chưa (tất cả wave đã ra xong và không còn enemy)
        if (!isGameEnded)
        {
            // Nếu EnemySpawner tồn tại thì dùng biến IsSpawningFinished, nếu không mặc định chờ hết enemy.
            bool isSpawningFinished = EnemySpawner.Instance == null || EnemySpawner.Instance.IsSpawningFinished;
            
            if (isSpawningFinished && EnemyHealth.LivingEnemyCount <= 0 && Time.timeSinceLevelLoad > 1f)
            {
                OnGameWin();
            }
        }
    }

    /// <summary>
    /// Gọi khi player chết - hiển thị Game Over
    /// </summary>
    private void OnGameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("=== GAME OVER ===");

        // Hiển thị Game Over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Tắt nhạc nền
        if (bgMusic != null)
        {
            bgMusic.SetActive(false);
        }
    }

    /// <summary>
    /// Gọi khi tất cả enemy bị tiêu diệt - hiển thị Game Win
    /// </summary>
    private void OnGameWin()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("=== GAME WIN ===");

        // Hiển thị Game Win UI
        if (gameWinUI != null)
        {
            gameWinUI.SetActive(true);
        }

        // Tắt nhạc nền
        if (bgMusic != null)
        {
            bgMusic.SetActive(false);
        }

        // Ẩn player
        if (playerHealth != null)
        {
            playerHealth.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Quay về Main Menu - gán vào Button trong UI
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Chơi lại - gán vào Button trong UI
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
