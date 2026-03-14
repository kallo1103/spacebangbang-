using UnityEngine;

/// <summary>
/// Đánh dấu một điểm trên đường bay (FlyPath) của Enemy.
/// Sử dụng Gizmos để hiển thị vị trí trong Scene view (không ảnh hưởng game runtime).
/// 
/// Cách dùng:
///   1. Tạo Empty GameObject, gắn script này
///   2. Đặt ở vị trí mong muốn trên đường bay
///   3. Nhóm tất cả waypoints dưới 1 parent có FlyPath component
/// </summary>
public class Waypoint : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [Tooltip("Màu sắc của Gizmo sphere hiển thị trong Scene view")]
    public Color gizmoColor = Color.green;

    [Tooltip("Kích thước Gizmo sphere")]
    public float gizmoRadius = 0.1f;

    /// <summary>
    /// Vẽ Gizmo luôn hiển thị (không cần select object).
    /// Giúp dễ dàng nhìn thấy tất cả waypoints khi thiết kế đường bay.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
