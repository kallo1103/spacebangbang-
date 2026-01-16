using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ExampleShipControl : MonoBehaviour {

	[Header("Movement Settings")]
	public float moveSpeed = 8f;          // Tốc độ bay
	public float rotationSpeed = 15f;     // Tốc độ xoay đầu

	private Rigidbody2D rb;

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
	}
}
