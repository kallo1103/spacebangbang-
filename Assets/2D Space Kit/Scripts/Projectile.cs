using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public GameObject shoot_effect;
	public GameObject hit_effect;
	public GameObject firing_ship;
	
	private bool isInitialized = false;
	private bool canCollide = false;  // Delay để tránh va chạm ngay khi spawn
	private float spawnTime;
	
	// Use this for initialization
	void Start () {
		spawnTime = Time.time;
		// Chỉ spawn muzzle flash nếu đã được khởi tạo đúng cách
		if (isInitialized) {
			SpawnMuzzleFlash();
		}
		Destroy(gameObject, 5f); //Bullet will despawn after 5 seconds
	}
	
	void Update() {
		// Đợi 0.1 giây sau khi spawn mới cho phép va chạm
		if (!canCollide && Time.time - spawnTime > 0.1f) {
			canCollide = true;
		}
	}
	
	/// <summary>
	/// Gọi method này SAU KHI gán firing_ship để spawn muzzle flash
	/// </summary>
	public void Initialize(GameObject ship) {
		firing_ship = ship;
		isInitialized = true;
		SpawnMuzzleFlash();
	}
	
	private void SpawnMuzzleFlash() {
		if (shoot_effect != null && firing_ship != null) {
			GameObject obj = (GameObject) Instantiate(shoot_effect, transform.position - new Vector3(0,0,5), Quaternion.identity);
			obj.transform.parent = firing_ship.transform;
		}
	}
	
	void OnTriggerEnter2D(Collider2D col) {
		// Chưa đủ thời gian delay thì bỏ qua
		if (!canCollide) return;
		
		// Không va chạm với firing_ship và các child của nó (Player, Turret, BarrelPoint...)
		if (firing_ship != null && IsChildOf(col.transform, firing_ship.transform)) {
			return;
		}
		
		// Không va chạm với các projectile khác
		if (col.gameObject.tag == "Projectile") {
			return;
		}
		
		// Va chạm hợp lệ - spawn hiệu ứng và destroy
		if (hit_effect != null) {
			Instantiate(hit_effect, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
	
	/// <summary>
	/// Kiểm tra xem child có phải là con/cháu của parent không
	/// </summary>
	private bool IsChildOf(Transform child, Transform parent) {
		if (child == parent) return true;
		
		Transform current = child;
		while (current != null) {
			if (current == parent) return true;
			current = current.parent;
		}
		return false;
	}
}

