using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Quản lý máu và trạng thái sống/chết của Player
/// Kế thừa từ Health base class, thêm invincibility và flash effect
/// </summary>
public class PlayerHealth : Health
{
    [Header("Invincibility")]
    public float invincibilityDuration = 1f;  // Thời gian bất tử sau khi bị đánh

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged;  // (currentHealth, maxHealth)
    public UnityEvent OnPlayerDied;

    // Properties
    public float CurrentHealth => currentHealth;
    public float MaxHealth => defaultHealthPoint;
    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible;

    // Private variables
    private bool isDead = false;
    private bool isInvincible = false;

    protected override void Start()
    {
        base.Start(); // Gọi Health.Start() để khởi tạo currentHealth và spriteRenderer

        // Thông báo UI về health ban đầu
        OnHealthChanged?.Invoke(currentHealth, defaultHealthPoint);
    }

    /// <summary>
    /// Override TakeDamage để thêm invincibility và flash
    /// </summary>
    public override void TakeDamage(int damage)
    {
        // Không nhận sát thương nếu đã chết hoặc đang bất tử
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{defaultHealthPoint}");

        // Thông báo UI
        OnHealthChanged?.Invoke(currentHealth, defaultHealthPoint);

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
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, defaultHealthPoint);

        OnHealthChanged?.Invoke(currentHealth, defaultHealthPoint);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        // Thông báo event cho các hệ thống khác
        OnPlayerDied?.Invoke();

        // Gọi base.Die() → spawn explosion, destroy, invoke onDead (cho BattleFlow)
        base.Die();
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
        currentHealth = defaultHealthPoint;
        isDead = false;
        isInvincible = false;
        gameObject.SetActive(true);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        OnHealthChanged?.Invoke(currentHealth, defaultHealthPoint);
    }
}
