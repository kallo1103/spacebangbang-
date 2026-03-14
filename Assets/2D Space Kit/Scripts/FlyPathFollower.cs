using UnityEngine;

/// <summary>
/// Enemy di chuyển theo đường bay FlyPath (waypoints).
/// Có thể dùng thay thế hoặc kết hợp với Enemy movement types.
/// 
/// Cách dùng:
///   1. Gắn script này vào Enemy prefab
///   2. Kéo FlyPath object vào field flyPath
///   3. Enemy sẽ tự động bay theo các waypoints khi game chạy
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class FlyPathFollower : MonoBehaviour
{
    [Header("Path")]
    [Tooltip("FlyPath chứa các waypoint mà enemy sẽ bay theo")]
    public FlyPath flyPath;

    [Header("Movement")]
    [Tooltip("Tốc độ di chuyển (nếu = 0 thì dùng defaultSpeed của FlyPath)")]
    public float moveSpeed = 0f;

    [Tooltip("Khoảng cách đến waypoint để coi là đã đến nơi")]
    public float reachDistance = 0.1f;

    [Tooltip("Xoay enemy theo hướng di chuyển")]
    public bool rotateTowardsMovement = true;

    [Tooltip("Tốc độ xoay (độ/giây). Giá trị cao = xoay nhanh")]
    public float rotationSpeed = 360f;

    // ─── Private ──────────────────────────────────────────────────
    private int currentWaypointIndex = 0;
    private bool pathCompleted = false;

    // ─── Lifecycle ────────────────────────────────────────────────
    void Start()
    {
        // Dùng tốc độ mặc định của FlyPath nếu chưa gán
        if (moveSpeed <= 0f && flyPath != null)
        {
            moveSpeed = flyPath.defaultSpeed;
        }

        // Đặt enemy ở vị trí waypoint đầu tiên
        if (flyPath != null && flyPath.WaypointCount > 0)
        {
            transform.position = flyPath.GetWaypointPosition(0);
            currentWaypointIndex = 1; // Bắt đầu di chuyển đến waypoint thứ 2
        }
    }

    void Update()
    {
        if (flyPath == null || flyPath.WaypointCount == 0 || pathCompleted) return;

        MoveTowardsWaypoint();
    }

    // ─── Private Methods ──────────────────────────────────────────
    private void MoveTowardsWaypoint()
    {
        Vector3 targetPos = flyPath.GetWaypointPosition(currentWaypointIndex);
        Vector3 direction = (targetPos - transform.position).normalized;

        // Di chuyển về phía waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Xoay theo hướng di chuyển
        if (rotateTowardsMovement && direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Kiểm tra đã đến waypoint chưa
        if (Vector3.Distance(transform.position, targetPos) <= reachDistance)
        {
            AdvanceToNextWaypoint();
        }
    }

    private void AdvanceToNextWaypoint()
    {
        currentWaypointIndex++;

        // Đã đi hết waypoints
        if (currentWaypointIndex >= flyPath.WaypointCount)
        {
            if (flyPath.loop)
            {
                // Quay lại waypoint đầu tiên
                currentWaypointIndex = 0;
            }
            else
            {
                // Kết thúc path - tùy chọn destroy hoặc dừng
                pathCompleted = true;
                OnPathCompleted();
            }
        }
    }

    /// <summary>
    /// Gọi khi enemy đi hết đường bay (không loop).
    /// Override hoặc mở rộng để thêm behaviour khác.
    /// </summary>
    protected virtual void OnPathCompleted()
    {
        // Mặc định: tiếp tục bay thẳng xuống sau khi hết path
        // Hoặc có thể tự hủy
        Debug.Log($"{gameObject.name}: Path completed. Continuing downward.");
    }
}
