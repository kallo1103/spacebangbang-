using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton quản lý điểm số trong game
/// Game Over/Win logic được xử lý bởi BattleFlow
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    public int currentScore = 0;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged;

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
        currentScore = 0;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Cộng điểm khi tiêu diệt enemy
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        Debug.Log($"Score: {currentScore} (+{points})");

        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// Restart game
    /// </summary>
    public void RestartGame()
    {
        currentScore = 0;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Thoát game
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
