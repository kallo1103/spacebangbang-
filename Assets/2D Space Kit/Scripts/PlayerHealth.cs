using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Quản lý máu và trạng thái sống/chết của Player.
/// Kế thừa từ Health base class, thêm invincibility frames và flash effect.
/// 
/// Sử dụng 2 hệ thống event song song:
///   - Health.onHealthChanged (System.Action) → cho HealthBar UI (mask-based)
///   - OnHealthChanged (UnityEvent)          → cho HealthUI (fill-based)
/// </summary>
public class PlayerHealth : Health
{
    [Header("Invincibility")]
    public float invincibilityDuration = 1f;  // Thời gian bất tử sau khi bị đánh

    [Header("Unity Events (Inspector)")]
    public UnityEvent<float, float> OnHealthChanged;  // (currentHealth, maxHealth)
    public UnityEvent OnPlayerDied;

    // Properties
    public float CurrentHealth => healthPoint;
    public float MaxHealth => defaultHealthPoint;
    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible;

    // Private variables
    private bool isDead = false;
    private bool isInvincible = false;

    protected override void Start()
    {
        base.Start(); // Health.Start() → khởi tạo healthPoint, invoke onHealthChanged

        // Thông báo UI qua UnityEvent
        OnHealthChanged?.Invoke(healthPoint, defaultHealthPoint);
    }

    /// <summary>
    /// Override TakeDamage để thêm invincibility và flash.
    /// Gọi base.onHealthChanged (cho HealthBar) và OnHealthChanged (cho HealthUI).
    /// </summary>
    public override void TakeDamage(int damage)
    {
        // Không nhận sát thương nếu đã chết hoặc đang bất tử
        if (isDead || isInvincible) return;

        healthPoint -= damage;
        healthPoint = Mathf.Max(0, healthPoint);

        Debug.Log($"Player took {damage} damage. Health: {healthPoint}/{defaultHealthPoint}");

        // Thông báo HealthBar (mask-based) qua System.Action
        onHealthChanged?.Invoke();

        // Thông báo HealthUI (fill-based) qua UnityEvent
        OnHealthChanged?.Invoke(healthPoint, defaultHealthPoint);

        // Hiệu ứng flash
        StartCoroutine(FlashRed());

        // Bật invincibility frames
        StartCoroutine(InvincibilityFrames());

        // Kiểm tra chết
        if (healthPoint <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Hồi máu cho player.
    /// </summary>
    public override void Heal(int amount)
    {
        if (isDead) return;

        healthPoint += amount;
        healthPoint = Mathf.Min(healthPoint, defaultHealthPoint);

        onHealthChanged?.Invoke();
        OnHealthChanged?.Invoke(healthPoint, defaultHealthPoint);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        // Thông báo event cho các hệ thống khác
        OnPlayerDied?.Invoke();

        // Gọi base.Die() → spawn explosion, invoke onDead (cho BattleFlow), destroy
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
    /// Reset lại health (dùng khi restart game).
    /// </summary>
    public void ResetHealth()
    {
        healthPoint = defaultHealthPoint;
        isDead = false;
        isInvincible = false;
        gameObject.SetActive(true);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        onHealthChanged?.Invoke();
        OnHealthChanged?.Invoke(healthPoint, defaultHealthPoint);
    }
}
