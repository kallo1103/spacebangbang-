using UnityEngine;

/// <summary>
/// Base class cho tất cả object có health (Player, Enemy, ...)
/// Cung cấp hệ thống máu, event onDead, và hiệu ứng nổ khi chết
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public GameObject explosionPrefab;    // Prefab hiệu ứng nổ khi chết
    public int defaultHealthPoint = 100;  // Máu mặc định

    [Header("Sound")]
    public AudioClip explosionSound;      // Âm thanh nổ khi chết

    /// <summary>
    /// Event được gọi khi object chết - BattleFlow sẽ lắng nghe event này
    /// </summary>
    public System.Action onDead;

    // Internal
    protected int currentHealth;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        currentHealth = defaultHealthPoint;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Gây sát thương cho object
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Hiệu ứng flash đỏ khi bị trúng đạn
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Xử lý khi chết - có thể override trong class con
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

        // Hủy object
        Destroy(gameObject);

        // Gọi event onDead để thông báo cho BattleFlow
        onDead?.Invoke();
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
