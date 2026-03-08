using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hiển thị health của player trên UI
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;       // Kéo Player có PlayerHealth vào đây
    
    [Header("UI Elements")]
    public Image healthBarFill;             // Image fill của health bar
    public TextMeshProUGUI healthText;      // Text hiển thị số máu (optional)
    
    [Header("Colors")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.3f;  // Dưới 30% thì đổi màu đỏ
    
    void Start()
    {
        // Tự tìm PlayerHealth nếu chưa gán
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }
        
        // Subscribe vào event
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
            
            // Update UI ban đầu
            UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
        else
        {
            Debug.LogWarning("HealthUI: Không tìm thấy PlayerHealth!");
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe khi destroy
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }
    
    /// <summary>
    /// Update UI khi health thay đổi
    /// </summary>
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        float healthPercent = currentHealth / maxHealth;
        
        // Update health bar fill
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = healthPercent;
            
            // Đổi màu theo lượng máu
            healthBarFill.color = healthPercent <= lowHealthThreshold 
                ? lowHealthColor 
                : Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }
        
        // Update text
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)} / {maxHealth}";
        }
    }
}
