using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerControlledTurret : MonoBehaviour {

	public GameObject weapon_prefab;
	public GameObject[] barrel_hardpoints;
	public float turret_rotation_speed = 3f;
	public float shot_speed;
	int barrel_index = 0;

	[Header("Sound")]
	public AudioClip shootSound;          // Kéo file SFX tiếng bắn vào đây
	private AudioSource audioSource;
	
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null) {
			audioSource = gameObject.AddComponent<AudioSource>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Kiểm tra input system
		if (Pointer.current == null || Mouse.current == null) return;
		
		// Lấy vị trí chuột từ New Input System
		Vector2 mousePosition = Pointer.current.position.ReadValue();
		
		// Turret xoay theo chuột
		Vector2 turretPosition = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 direction = (Vector3)mousePosition - (Vector3)turretPosition;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.LerpAngle(transform.rotation.eulerAngles.z, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f, turret_rotation_speed * Time.deltaTime)));


		// Bắn khi click chuột trái (New Input System)
		if (Mouse.current.leftButton.wasPressedThisFrame && barrel_hardpoints != null && barrel_hardpoints.Length > 0) {
			// Phát âm thanh bắn
			if (shootSound != null && audioSource != null) {
				audioSource.PlayOneShot(shootSound);
			}

			GameObject bullet = (GameObject) Instantiate(weapon_prefab, barrel_hardpoints[barrel_index].transform.position, transform.rotation);
			
			Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
			if (bulletRb != null) {
				bulletRb.AddForce(bullet.transform.up * shot_speed);
			}
			
			// Sử dụng Initialize() để gán firing_ship và spawn muzzle flash đúng cách
			Projectile projectileScript = bullet.GetComponent<Projectile>();
			if (projectileScript != null) {
				projectileScript.Initialize(transform.parent.gameObject);
			}
			
			barrel_index++; //This will cycle sequentially through the barrels in the barrel_hardpoints array
			
			if (barrel_index >= barrel_hardpoints.Length)
				barrel_index = 0;
		}
	
	}
}

