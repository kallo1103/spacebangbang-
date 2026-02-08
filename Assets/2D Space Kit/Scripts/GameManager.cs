using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton quản lý trạng thái game: điểm số, game over, restart
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    public bool isGameOver = false;
    
    [Header("Score")]
    public int currentScore = 0;
    
    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent OnGameOver;
    public UnityEvent OnGameRestart;
    
    [Header("UI References (Optional)")]
    public GameObject gameOverPanel;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        // Đảm bảo game bắt đầu bình thường
        isGameOver = false;
        Time.timeScale = 1f;
        
        // Ẩn game over panel nếu có
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Cộng điểm khi tiêu diệt enemy
    /// </summary>
    public void AddScore(int points)
    {
        if (isGameOver) return;
        
        currentScore += points;
        Debug.Log($"Score: {currentScore} (+{points})");
        
        OnScoreChanged?.Invoke(currentScore);
    }
    
    /// <summary>
    /// Gọi khi player chết
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        
        Debug.Log($"GAME OVER! Final Score: {currentScore}");
        
        // Có thể dừng game hoặc chậm lại
        // Time.timeScale = 0f;  // Uncomment nếu muốn pause hoàn toàn
        
        // Hiển thị UI game over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        OnGameOver?.Invoke();
    }
    
    /// <summary>
    /// Restart game - gọi từ UI button
    /// </summary>
    public void RestartGame()
    {
        isGameOver = false;
        currentScore = 0;
        Time.timeScale = 1f;
        
        OnGameRestart?.Invoke();
        
        // Reload scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Thoát game - gọi từ UI button
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
