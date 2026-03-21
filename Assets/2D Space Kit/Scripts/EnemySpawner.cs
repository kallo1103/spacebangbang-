using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý việc spawn theo danh sách các wave (từ ScriptableObject LevelWaveData).
/// Có thể setup số lượng enemy, offset giữa các enemy và FlyPath riêng cho từng wave.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // Singleton để dễ dàng truy cập từ BattleFlow nếu cần
    public static EnemySpawner Instance { get; private set; }

    [Header("Level Waves")]
    public LevelWaveData levelData;   // Kéo LevelWaveData Scriptable Object vào đây
    public FlyPath[] paths;           // Danh sách các FlyPath có sẵn trong scene

    // Báo hiệu spawner đã hoàn tất việc sinh ra tất cả wave
    public bool IsSpawningFinished { get; private set; }

    private int currentWaveIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IsSpawningFinished = false;

        if (levelData != null && levelData.enemyWaves.Length > 0)
        {
            StartCoroutine(SpawnEnemyWaves());
        }
        else
        {
            Debug.LogWarning("Chưa gán LevelWaveData cho EnemySpawner! Vui lòng tạo LevelWaveData và kéo vào.");
            IsSpawningFinished = true;
        }
    }

    private IEnumerator SpawnEnemyWaves()
    {
        while (currentWaveIndex < levelData.enemyWaves.Length)
        {
            var waveInfo = levelData.enemyWaves[currentWaveIndex];

            // Tìm path tương ứng cho wave này
            FlyPath wavePath = null;
            if (waveInfo.pathIndex >= 0 && waveInfo.pathIndex < paths.Length)
            {
                wavePath = paths[waveInfo.pathIndex];
            }

            // Vị trí spawn ban đầu của wave (lấy điểm đầu tiên trên path, hoặc vị trí spawner)
            Vector3 startPosition = wavePath != null ? wavePath.GetWaypointPosition(0) : transform.position;

            // Spawn lần lượt số lượng enemy khai báo
            for (int i = 0; i < waveInfo.numberOfEnemy; i++)
            {
                if (waveInfo.enemyPrefab != null)
                {
                    GameObject enemy = Instantiate(waveInfo.enemyPrefab, startPosition, Quaternion.identity);

                    // Gán path và tốc độ di chuyển
                    FlyPathFollower agent = enemy.GetComponent<FlyPathFollower>();
                    if (agent != null)
                    {
                        if (wavePath != null) agent.flyPath = wavePath;
                        if (waveInfo.speed > 0f) agent.moveSpeed = waveInfo.speed;
                    }
                }

                // Dịch chuyển vị trí spawn cho enemy tiếp theo trong cùng 1 wave
                startPosition += waveInfo.formationOffset;
            }

            currentWaveIndex++;

            if (currentWaveIndex < levelData.enemyWaves.Length)
            {
                yield return new WaitForSeconds(waveInfo.nextWaveDelay);
            }
        }

        // Hoàn tất spawn mọi wave trong màn chơi
        IsSpawningFinished = true;
        Debug.Log("Đã spawn toàn bộ enemy waves.");
    }
}
