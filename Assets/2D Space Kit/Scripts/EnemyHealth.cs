using UnityEngine;

/// <summary>
/// Health cho Enemy - kế thừa Health base class
/// Đếm số enemy còn sống bằng static LivingEnemyCount
/// BattleFlow sẽ kiểm tra LivingEnemyCount <= 0 để trigger Game Win
/// </summary>
public class EnemyHealth : Health
{
    /// <summary>
    /// Số enemy còn sống trong scene - dùng để kiểm tra thắng game
    /// </summary>
    public static int LivingEnemyCount;

    [Header("Score")]
    public int scoreValue = 100;  // Điểm nhận được khi tiêu diệt enemy này

    private void Awake()
    {
        // Mỗi khi enemy được tạo ra, tăng bộ đếm
        LivingEnemyCount++;
        Debug.Log($"Enemy spawned. Living enemies: {LivingEnemyCount}");
    }

    protected override void Die()
    {
        // Giảm bộ đếm trước khi chết
        LivingEnemyCount--;
        Debug.Log($"Enemy killed. Living enemies: {LivingEnemyCount}");

        // Cộng điểm khi tiêu diệt
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Gọi base.Die() để spawn explosion, destroy, và invoke onDead
        base.Die();
    }
}
