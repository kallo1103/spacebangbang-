using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ExampleShipControl : MonoBehaviour {

	[Header("Movement Settings")]
	public float moveSpeed = 8f;          // Tốc độ bay
	public float rotationSpeed = 15f;     // Tốc độ xoay đầu

	[Header("Shooting Settings")]
	public GameObject projectilePrefab;   // Prefab viên đạn (kéo Projectile Sharp vào đây)
	public Transform firePoint;           // Vị trí bắn (tạo empty child object ở đầu tàu)
	public float projectileSpeed = 800f;  // Tốc độ đạn bay
	public float fireRate = 0.2f;         // Thời gian giữa các lần bắn (giây)
	
	private Rigidbody2D rb;
	private float nextFireTime = 0f;      // Thời điểm có thể bắn tiếp theo

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		// Đảm bảo không bị rơi do trọng lực
		rb.gravityScale = 0; 
	}
	
	void Update () {
		// Kiểm tra hệ thống nhập liệu
		if (Keyboard.current == null || Pointer.current == null) return;

		// --- 1. DI CHUYỂN (WASD) ---
		float moveX = 0f;
		float moveY = 0f;

		if (Keyboard.current.wKey.isPressed) moveY = 1f;
		if (Keyboard.current.sKey.isPressed) moveY = -1f;
		if (Keyboard.current.aKey.isPressed) moveX = -1f;
		if (Keyboard.current.dKey.isPressed) moveX = 1f;

		// Tạo Vector hướng di chuyển (Normalized để không đi chéo nhanh hơn)
		Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

		// Gán vận tốc trực tiếp (Kiểu Arcade: Thả phím là dừng ngay)
		rb.linearVelocity = moveDirection * moveSpeed;


		// --- 2. XOAY THEO CHUỘT (AIM) ---
		// Lấy vị trí chuột trong thế giới game
		Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
		
		// Tính góc xoay
		Vector2 direction = (Vector3)mouseWorldPos - transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // -90 vì sprite gốc hướng lên
		Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		
		// Xoay máy bay
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

		// --- 3. BẮN ĐẠN (CLICK CHUỘT TRÁI) ---
		if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime) {
			Shoot();
			nextFireTime = Time.time + fireRate;
		}
	}

	/// <summary>
	/// Hàm bắn đạn - Instantiate projectile và đẩy về phía trước
	/// </summary>
	void Shoot() {
		if (projectilePrefab == null) {
			Debug.LogWarning("Chưa gán Projectile Prefab! Vào Inspector và kéo prefab vào.");
			return;
		}

		// Xác định vị trí bắn (nếu không có firePoint thì dùng vị trí tàu)
		Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
		Quaternion spawnRotation = transform.rotation;

		// Tạo viên đạn
		GameObject bullet = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

		// Thêm lực đẩy viên đạn về phía trước (transform.up vì sprite hướng lên)
		Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
		if (bulletRb != null) {
			bulletRb.AddForce(transform.up * projectileSpeed);
		}

		// Gán firing_ship cho script Projectile (nếu có)
		Projectile projectileScript = bullet.GetComponent<Projectile>();
		if (projectileScript != null) {
			projectileScript.firing_ship = gameObject;
		}
	}
}

