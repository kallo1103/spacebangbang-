using UnityEngine;

/// <summary>
/// Script điều khiển Enemy - bao gồm health, AI movement, và hiệu ứng nổ
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public int scoreValue = 100;  // Điểm nhận khi tiêu diệt
    
    [Header("Movement")]
    public float moveSpeed = 3f;
    public MovementType movementType = MovementType.ChasePlayer;
    
    [Header("Effects")]
    public GameObject explosionPrefab;  // Kéo Explosion.prefab vào đây
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // Enums
    public enum MovementType
    {
        None,           // Đứng yên
        ChasePlayer,    // Đuổi theo player
        MoveDown,       // Bay xuống (classic shooter)
        Patrol          // Bay qua lại
    }
    
    // Private variables
    private float currentHealth;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Vector3 patrolStartPos;
    private float patrolDirection = 1f;
    public float patrolRange = 5f;
    
    [Header("Shooting")]
    public bool canShoot = false;           // Bật/tắt khả năng bắn
    public GameObject projectilePrefab;     // Prefab đạn enemy
    public float fireRate = 2f;             // Thời gian giữa các lần bắn
    public float projectileSpeed = 400f;    // Tốc độ đạn
    public float contactDamage = 25f;       // Sát thương khi đâm vào player
    
    private float nextFireTime;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        patrolStartPos = transform.position;
        
        // Tìm Player trong scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    void Update()
    {
        HandleMovement();
        HandleShooting();
    }
    
    /// <summary>
    /// Xử lý di chuyển dựa trên movement type
    /// </summary>
    private void HandleMovement()
    {
        switch (movementType)
        {
            case MovementType.ChasePlayer:
                ChasePlayer();
                break;
            case MovementType.MoveDown:
                MoveDown();
                break;
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.None:
            default:
                break;
        }
    }
    
    private void ChasePlayer()
    {
        if (playerTransform == null) return;
        
        // Hướng tới player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Xoay về phía player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void MoveDown()
    {
        // Di chuyển xuống dưới màn hình
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        
        // Tự hủy nếu ra khỏi màn hình
        if (transform.position.y < -15f)
        {
            Destroy(gameObject);
        }
    }
    
    private void Patrol()
    {
        // Di chuyển qua lại theo chiều ngang
        transform.position += Vector3.right * patrolDirection * moveSpeed * Time.deltaTime;
        
        // Đổi hướng khi đi quá phạm vi
        if (Mathf.Abs(transform.position.x - patrolStartPos.x) > patrolRange)
        {
            patrolDirection *= -1f;
        }
    }
    
    /// <summary>
    /// Xử lý bắn đạn về phía player
    /// </summary>
    private void HandleShooting()
    {
        if (!canShoot || projectilePrefab == null) return;
        if (playerTransform == null) return;
        
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    private void Shoot()
    {
        // Tạo đạn tại vị trí enemy, hướng về phía player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        
        GameObject bullet = Instantiate(projectilePrefab, transform.position, rotation);
        
        // Đẩy đạn về hướng player
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(directionToPlayer * projectileSpeed);
        }
        
        // Gán tag để Projectile biết đây là đạn enemy
        bullet.tag = "EnemyProjectile";
        
        // Gán firing_ship cho Projectile script
        Projectile projectileScript = bullet.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.firing_ship = gameObject;
            projectileScript.isEnemyProjectile = true;
        }
    }
    
    /// <summary>
    /// Gọi method này khi enemy bị trúng đạn
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        }
        
        // Hiệu ứng flash khi bị trúng đạn
        StartCoroutine(FlashRed());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            
            // Kiểm tra null vì object có thể đã bị destroy
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }
    
    private void Die()
    {
        // Spawn hiệu ứng nổ
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // Đảm bảo explosion có scale phù hợp
            explosion.transform.localScale = transform.localScale;
        }
        
        // TODO: Cộng điểm cho player (khi có GameManager)
        // GameManager.Instance.AddScore(scoreValue);
        
        if (showDebugInfo)
        {
            Debug.Log($"{gameObject.name} destroyed! Score: +{scoreValue}");
        }
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Va chạm trực tiếp với Player (kamikaze damage)
    /// </summary>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            // Gây sát thương cho player khi đâm vào
            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }
            
            // Tự hủy khi đâm vào player
            Die();
        }
    }
    
    /// <summary>
    /// Vẽ gizmos trong Editor để debug
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (movementType == MovementType.Patrol)
        {
            Gizmos.color = Color.yellow;
            Vector3 leftPoint = (Application.isPlaying ? patrolStartPos : transform.position) + Vector3.left * patrolRange;
            Vector3 rightPoint = (Application.isPlaying ? patrolStartPos : transform.position) + Vector3.right * patrolRange;
            Gizmos.DrawLine(leftPoint, rightPoint);
            Gizmos.DrawWireSphere(leftPoint, 0.3f);
            Gizmos.DrawWireSphere(rightPoint, 0.3f);
        }
    }
}
