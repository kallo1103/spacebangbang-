using UnityEngine;

/// <summary>
/// Cuộn texture background liên tục theo thời gian
/// Tạo hiệu ứng background chuyển động vô hạn
/// Dùng cho 3D Quad/Plane với Unlit shader
/// </summary>
public class ScrollingBackground : MonoBehaviour
{
    [Header("Settings")]
    public Renderer bgRenderer;     // Renderer của background object
    public float speed = 0.5f;      // Tốc độ cuộn

    void Update()
    {
        if (bgRenderer == null) return;

        // Cuộn texture offset theo thời gian
        bgRenderer.material.mainTextureOffset =
            new Vector2(0, Time.time * speed);
    }
}
