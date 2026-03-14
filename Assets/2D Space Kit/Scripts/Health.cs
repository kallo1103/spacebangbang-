using UnityEngine;

/// <summary>
/// Base class cho tất cả object có health (Player, Enemy, ...)
/// Cung cấp hệ thống máu, event onDead, onHealthChanged và hiệu ứng nổ khi chết.
/// 
/// HealthBar UI sẽ lắng nghe onHealthChanged để cập nhật thanh máu.
/// BattleFlow sẽ lắng nghe onDead để xử lý luồng game.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public GameObject explosionPrefab;    // Prefab hiệu ứng nổ khi chết
    public int defaultHealthPoint = 100;  // Máu mặc định

    [Header("Sound")]
    public AudioClip explosionSound;      // Âm thanh nổ khi chết

    // ─── Events ───────────────────────────────────────────────────
    /// <summary>
    /// Event được gọi khi object chết - BattleFlow sẽ lắng nghe event này
    /// </summary>
    public System.Action onDead;

    /// <summary>
    /// Event được gọi mỗi khi healthPoint thay đổi - HealthBar sẽ lắng nghe event này
    /// </summary>
    public System.Action onHealthChanged;

    // ─── Runtime State ────────────────────────────────────────────
    /// <summary>
    /// Máu hiện tại (public để HealthBar đọc được).
    /// Không nên gán trực tiếp từ bên ngoài, dùng TakeDamage / Heal thay vì.
    /// </summary>
    [HideInInspector] public int healthPoint;

    protected SpriteRenderer spriteRenderer;

    // ─── Lifecycle ────────────────────────────────────────────────
    protected virtual void Start()
    {
        healthPoint = defaultHealthPoint;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Thông báo cho HealthBar cập nhật lần đầu
        onHealthChanged?.Invoke();
    }

    // ─── Public API ───────────────────────────────────────────────
    /// <summary>
    /// Gây sát thương cho object.
    /// Tự động gọi onHealthChanged và kiểm tra chết.
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (healthPoint <= 0) return;

        healthPoint -= damage;
        healthPoint = Mathf.Max(0, healthPoint);

        // Thông báo cho HealthBar
        onHealthChanged?.Invoke();

        // Hiệu ứng flash đỏ khi bị trúng đạn
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (healthPoint <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Hồi máu cho object.
    /// </summary>
    public virtual void Heal(int amount)
    {
        if (healthPoint <= 0) return;

        healthPoint += amount;
        healthPoint = Mathf.Min(healthPoint, defaultHealthPoint);

        onHealthChanged?.Invoke();
    }

    // ─── Protected ────────────────────────────────────────────────
    /// <summary>
    /// Xử lý khi chết - có thể override trong class con.
    /// </summary>
    protected virtual void Die()
    {
        // Spawn hiệu ứng nổ
        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(explosion, 1f);
        }

        // Phát âm thanh nổ
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Gọi event onDead TRƯỚC KHI Destroy để listener vẫn nhận được
        onDead?.Invoke();

        // Hủy object
        Destroy(gameObject);
    }

    protected System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}
