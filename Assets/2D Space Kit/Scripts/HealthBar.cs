using UnityEngine;

/// <summary>
/// Hiển thị thanh máu bằng kỹ thuật Mask + 9-Sliced Sprite.
/// 
/// Cấu trúc Hierarchy:
///   HealthBarCanvas (Canvas - World Space hoặc Screen Space)
///   └── HealthBar (gắn script này)
///       ├── Background    (Image - 9-sliced health bar background)
///       ├── Mask           (RectTransform dùng làm RectMask2D)
///       │   └── Fill       (Image - 9-sliced thanh máu màu vàng/xanh)
///       └── HeartIcon      (Image - icon trái tim, optional)
///
/// Cách hoạt động:
///   - Khi health thay đổi, script thay đổi width của Mask RectTransform
///   - Fill image bên trong Mask sẽ bị cắt (mask) tương ứng với health còn lại
///   - Sử dụng 9-slice để thanh máu không bị stretch khi thay đổi kích thước
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("RectTransform của Mask object (chứa RectMask2D component)")]
    public RectTransform mask;

    [Tooltip("Health component cần theo dõi (Player hoặc Enemy)")]
    public Health health;

    // ─── Private ──────────────────────────────────────────────────
    private float originalWidth;

    // ─── Lifecycle ────────────────────────────────────────────────
    void Start()
    {
        // Lưu lại width ban đầu của Mask (khi health = 100%)
        originalWidth = mask.sizeDelta.x;

        // Cập nhật thanh máu lần đầu
        UpdateHealthValue();

        // Đăng ký lắng nghe event health thay đổi
        if (health != null)
        {
            health.onHealthChanged += UpdateHealthValue;
        }
    }

    void OnDestroy()
    {
        // Hủy đăng ký event để tránh memory leak
        if (health != null)
        {
            health.onHealthChanged -= UpdateHealthValue;
        }
    }

    // ─── Private Methods ──────────────────────────────────────────
    /// <summary>
    /// Tính toán và cập nhật width của Mask dựa trên tỷ lệ health hiện tại.
    /// </summary>
    private void UpdateHealthValue()
    {
        if (health == null || mask == null) return;

        // Tỷ lệ health hiện tại / health tối đa (0.0 ~ 1.0)
        float scale = (float)health.healthPoint / health.defaultHealthPoint;
        scale = Mathf.Clamp01(scale);

        // Chỉ thay đổi width, giữ nguyên height
        mask.sizeDelta = new Vector2(scale * originalWidth, mask.sizeDelta.y);
    }
}
