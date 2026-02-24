using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script điều khiển Main Menu
/// Xử lý chuyển scene khi nhấn Play button
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Gọi khi nhấn nút Play - chuyển sang scene Battle
    /// </summary>
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("Battle");
    }

    /// <summary>
    /// Gọi khi nhấn nút Quit - thoát game
    /// </summary>
    public void OnQuitButtonClicked()
    {
        Debug.Log("Quitting game...");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
