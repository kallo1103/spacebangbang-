using UnityEngine;

/// <summary>
/// Camera theo dõi Player với giới hạn vùng chơi
/// Player và Enemy luôn nằm trong khung hình
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                // Kéo Player vào đây
    
    [Header("Follow Settings")]
    public float smoothSpeed = 5f;          // Tốc độ camera đuổi theo
    public Vector3 offset = new Vector3(0, 0, -10);  // Offset (Z = -10 cho 2D)
    
    [Header("Boundaries - Giới hạn vùng chơi")]
    public bool useBoundaries = true;
    public float minX = -10f;               // Giới hạn trái
    public float maxX = 10f;                // Giới hạn phải
    public float minY = -8f;                // Giới hạn dưới
    public float maxY = 8f;                 // Giới hạn trên
    
    void Start()
    {
        // Tự tìm Player nếu chưa gán
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Vị trí mong muốn
        Vector3 desiredPosition = target.position + offset;
        
        // Giới hạn vị trí camera trong vùng chơi
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        // Di chuyển mượt
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    
    /// <summary>
    /// Vẽ vùng giới hạn trong Editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;
        
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
