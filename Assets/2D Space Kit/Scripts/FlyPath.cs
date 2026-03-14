using UnityEngine;

/// <summary>
/// Quản lý đường bay (fly path) cho Enemy.
/// Tự động thu thập tất cả Waypoint con khi bấm Reset trong Inspector.
/// 
/// Cách dùng:
///   1. Tạo Empty GameObject "FlyPath_01", gắn script này
///   2. Tạo các Empty GameObject con, gắn Waypoint script
///   3. Bấm chuột phải vào FlyPath component → Reset (hoặc thêm thủ công)
///   4. Enemy sẽ bay theo thứ tự các waypoint
///
/// Có thể vẽ đường nối giữa các waypoint trong Scene view để dễ thiết kế.
/// </summary>
public class FlyPath : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Danh sách các Waypoint theo thứ tự bay. Bấm Reset để tự động lấy từ children.")]
    public Waypoint[] waypoints;

    [Header("Path Settings")]
    [Tooltip("Tốc độ di chuyển mặc định trên path này")]
    public float defaultSpeed = 3f;

    [Tooltip("Lặp lại đường bay khi đến waypoint cuối")]
    public bool loop = false;

    // ─── Editor Utility ───────────────────────────────────────────
    /// <summary>
    /// Tự động tìm tất cả Waypoint children khi bấm Reset trong Inspector.
    /// Rất tiện để không phải kéo thả thủ công.
    /// </summary>
    private void Reset()
    {
        waypoints = GetComponentsInChildren<Waypoint>();
    }

    // ─── Public API ───────────────────────────────────────────────
    /// <summary>
    /// Lấy vị trí của waypoint theo index.
    /// </summary>
    public Vector3 GetWaypointPosition(int index)
    {
        if (waypoints == null || waypoints.Length == 0) return transform.position;

        index = Mathf.Clamp(index, 0, waypoints.Length - 1);
        return waypoints[index].transform.position;
    }

    /// <summary>
    /// Tổng số waypoints trong path.
    /// </summary>
    public int WaypointCount => waypoints != null ? waypoints.Length : 0;

    // ─── Gizmos ───────────────────────────────────────────────────
    /// <summary>
    /// Vẽ đường nối giữa các waypoint để dễ nhìn trong Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(
                    waypoints[i].transform.position,
                    waypoints[i + 1].transform.position
                );
            }
        }

        // Nếu loop, vẽ thêm đường từ waypoint cuối về waypoint đầu
        if (loop && waypoints.Length > 1)
        {
            var first = waypoints[0];
            var last = waypoints[waypoints.Length - 1];
            if (first != null && last != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(last.transform.position, first.transform.position);
            }
        }
    }
}
