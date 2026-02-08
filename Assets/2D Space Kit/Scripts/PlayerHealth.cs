using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Quản lý máu và trạng thái sống/chết của Player
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    
    [Header("Effects")]
    public GameObject deathExplosionPrefab;  // Hiệu ứng nổ khi chết
    
    [Header("Invincibility")]
    public float invincibilityDuration = 1f;  // Thời gian bất tử sau khi bị đánh
    
    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged;  // (currentHealth, maxHealth)
    public UnityEvent OnPlayerDied;
    
    // Properties
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible;
    
    // Private variables
    private float currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Thông báo UI về health ban đầu
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// Gây sát thương cho player
    /// </summary>
    public void TakeDamage(float damage)
    {
        // Không nhận sát thương nếu đã chết hoặc đang bất tử
        if (isDead || isInvincible) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);  // Không cho âm
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // Thông báo UI
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Hiệu ứng flash
        StartCoroutine(FlashRed());
        
        // Bật invincibility frames
        StartCoroutine(InvincibilityFrames());
        
        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Hồi máu cho player
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);  // Không vượt max
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("Player died!");
        
        // Spawn hiệu ứng nổ
        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }
        
        // Thông báo cho các hệ thống khác (GameManager, UI...)
        OnPlayerDied?.Invoke();
        
        // Thông báo GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        // Ẩn player (hoặc có thể Destroy nếu muốn)
        gameObject.SetActive(false);
    }
    
    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }
    
    private System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        
        // Nhấp nháy để thể hiện bất tử
        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        
        // Đảm bảo sprite hiển thị lại
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        isInvincible = false;
    }
    
    /// <summary>
    /// Reset lại health (dùng khi restart game)
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInvincible = false;
        gameObject.SetActive(true);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
