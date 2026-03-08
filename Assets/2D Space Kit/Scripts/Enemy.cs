using UnityEngine;

/// <summary>
/// Script điều khiển Enemy - bao gồm AI movement và shooting
/// Health được quản lý bởi EnemyHealth component (add cùng GameObject)
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public MovementType movementType = MovementType.ChasePlayer;

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
    private Transform playerTransform;
    private Vector3 patrolStartPos;
    private float patrolDirection = 1f;
    public float patrolRange = 5f;

    [Header("Shooting")]
    public bool canShoot = false;
    public GameObject projectilePrefab;
    public float fireRate = 2f;
    public float projectileSpeed = 400f;
    public float contactDamage = 25f;

    private float nextFireTime;

    void Start()
    {
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

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void MoveDown()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (transform.position.y < -15f)
        {
            Destroy(gameObject);
        }
    }

    private void Patrol()
    {
        transform.position += Vector3.right * patrolDirection * moveSpeed * Time.deltaTime;

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
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject bullet = Instantiate(projectilePrefab, transform.position, rotation);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(directionToPlayer * projectileSpeed);
        }

        bullet.tag = "EnemyProjectile";

        Projectile projectileScript = bullet.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.firing_ship = gameObject;
            projectileScript.isEnemyProjectile = true;
        }
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
                playerHealth.TakeDamage((int)contactDamage);
            }

            // Tự hủy thông qua EnemyHealth để đảm bảo LivingEnemyCount giảm
            EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(9999);
            }
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
