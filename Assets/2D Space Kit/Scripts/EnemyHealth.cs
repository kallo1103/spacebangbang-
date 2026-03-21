using UnityEngine;

/// <summary>
/// Health cho Enemy - kế thừa Health base class.
/// Đếm số enemy còn sống bằng static LivingEnemyCount.
/// BattleFlow sẽ kiểm tra LivingEnemyCount <= 0 để trigger Game Win.
/// 
/// HealthBar của enemy sử dụng Health.onHealthChanged event.
/// </summary>
public class EnemyHealth : Health
{
    /// <summary>
    /// Số enemy còn sống trong scene - dùng để kiểm tra thắng game.
    /// </summary>
    public static int LivingEnemyCount;

    [Header("Score")]
    public int scoreValue = 100;  // Điểm nhận được khi tiêu diệt enemy này

    private bool isDead = false;

    private void Awake()
    {
        // Mỗi khi enemy được tạo ra, tăng bộ đếm
        LivingEnemyCount++;
        Debug.Log($"Enemy spawned. Living enemies: {LivingEnemyCount}");
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        // Giảm bộ đếm trước khi chết
        LivingEnemyCount--;
        Debug.Log($"Enemy killed. Living enemies: {LivingEnemyCount}");

        // Cộng điểm khi tiêu diệt
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Gọi base.Die() để spawn explosion, invoke onDead & onHealthChanged, destroy
        base.Die();
    }

    private void OnDestroy()
    {
        // Đảm bảo giảm bộ đếm nếu enemy bị Destroy trực tiếp (VD: bay ra khỏi màn hình)
        if (!isDead)
        {
            isDead = true;
            LivingEnemyCount--;
            Debug.Log($"Enemy destroyed. Living enemies: {LivingEnemyCount}");
        }
    }
}
