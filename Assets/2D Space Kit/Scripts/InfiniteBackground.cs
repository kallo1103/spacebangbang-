using UnityEngine;

public class InfiniteBackground : MonoBehaviour {
    
    [Tooltip("Gán Main Camera vào đây (hoặc để trống sẽ tự tìm)")]
    public Transform targetCamera;
    
    [Tooltip("Tốc độ di chuyển của nền so với camera (0-1). 1 là đứng yên cùng camera, nhỏ hơn 1 là nền xa.")]
    public float parallaxEffect = 0.5f;

    private MeshRenderer meshRenderer;
    private Material mat;
    private Vector2 offset;

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;

        if (targetCamera == null) {
            targetCamera = Camera.main.transform;
        }
    }

    void LateUpdate() {
        if (targetCamera == null) return;

        // Tính toán độ dời dựa trên vị trí camera
        Vector3 camPos = targetCamera.position;
        
        // Di chuyển MainTextureOffset của vật liệu
        // Chia cho scale của object để texture không bị méo nếu scale lớn
        offset.x = camPos.x * parallaxEffect / transform.localScale.x;
        offset.y = camPos.y * parallaxEffect / transform.localScale.y;

        mat.mainTextureOffset = offset;
        
        // Giữ vị trí của Background luôn đi theo Camera (nhưng texture thì trôi)
        transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);
    }
}
