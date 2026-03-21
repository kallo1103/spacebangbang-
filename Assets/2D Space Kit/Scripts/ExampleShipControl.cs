using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ExampleShipControl : MonoBehaviour {

	[Header("Movement Settings")]
	public float moveSpeed = 8f;          // Tốc độ bay
	public float rotationSpeed = 15f;     // Tốc độ xoay đầu
	public float touchYOffset = 1.0f;     // Khoảng cách ship nằm trên ngón tay khi chạm

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
		// --- 1. DI CHUYỂN BẰNG BÀN PHÍM (NẾU CÓ) ---
		float moveX = 0f;
		float moveY = 0f;

		if (Keyboard.current != null) {
			if (Keyboard.current.wKey.isPressed) moveY = 1f;
			if (Keyboard.current.sKey.isPressed) moveY = -1f;
			if (Keyboard.current.aKey.isPressed) moveX = -1f;
			if (Keyboard.current.dKey.isPressed) moveX = 1f;
		}

		Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
		
		if (moveDirection.magnitude > 0) {
			rb.linearVelocity = moveDirection * moveSpeed;
		} else {
			rb.linearVelocity = Vector2.zero;
		}

		// --- 2. DI CHUYỂN & CHUẨN ĐÍCH THEO CẢM ỨNG / CHUỘT ---
		if (Pointer.current != null) {
			bool isPressing = Pointer.current.press.isPressed;
			Vector2 screenPos = Pointer.current.position.ReadValue();
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
			worldPos.z = 0f;

			if (isPressing) {
				// Di chuyển phi thuyền bay theo điểm chạm (Move Towards) để không bị teleport
				// Move above finger (nằm trên ngón tay) khi chơi ở điện thoại
				if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
				{
					worldPos.y += touchYOffset;
				}
				
				// Di chuyển mượt về phía vị trí chỉ định
				transform.position = Vector3.MoveTowards(transform.position, worldPos, moveSpeed * 1.5f * Time.deltaTime);

				// Xoay đầu hướng về phía chạm / điểm đến di chuyển (nếu không ở ngay tại điểm)
				Vector2 direction = (Vector3)worldPos - transform.position;
				if (direction.sqrMagnitude > 0.05f) {
					float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
					Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
				}

				// --- 3. AUTO BẮN KHI ĐANG CHẠM ---
				if (Time.time >= nextFireTime) {
					Shoot();
					nextFireTime = Time.time + fireRate;
				}
			}
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

