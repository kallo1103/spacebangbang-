using UnityEngine;

/// <summary>
/// Script để spawn enemy tự động trong game
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;       // Danh sách enemy prefabs
    public float spawnInterval = 2f;        // Thời gian giữa các lần spawn
    public float spawnDelay = 1f;           // Delay trước khi bắt đầu spawn
    
    [Header("Spawn Area")]
    public float spawnRangeX = 8f;          // Phạm vi spawn theo X (giảm để nằm trong camera)
    public float spawnPositionY = 5f;       // Vị trí Y spawn (giảm để nằm trong camera)
    
    [Header("Wave Settings")]
    public bool useWaves = false;           // Bật chế độ wave
    public int enemiesPerWave = 5;          // Số enemy mỗi wave
    public float waveCooldown = 5f;         // Thời gian nghỉ giữa các wave
    
    // Private variables
    private float nextSpawnTime;
    private int currentWaveSpawnCount;
    private bool isWaveCooldown;
    
    void Start()
    {
        nextSpawnTime = Time.time + spawnDelay;
        currentWaveSpawnCount = 0;
        isWaveCooldown = false;
    }
    
    void Update()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        
        if (Time.time >= nextSpawnTime && !isWaveCooldown)
        {
            SpawnEnemy();
            
            if (useWaves)
            {
                currentWaveSpawnCount++;
                if (currentWaveSpawnCount >= enemiesPerWave)
                {
                    // Bắt đầu wave cooldown
                    StartCoroutine(WaveCooldown());
                }
            }
            
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    private void SpawnEnemy()
    {
        // Random vị trí X trong phạm vi
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnPositionY, 0);
        
        // Random chọn enemy prefab
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0, 0, 180)); // Xoay 180 để hướng xuống
        
        // Random enemy settings một chút để đa dạng
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.moveSpeed *= Random.Range(0.8f, 1.2f);
        }
    }
    
    private System.Collections.IEnumerator WaveCooldown()
    {
        isWaveCooldown = true;
        Debug.Log($"Wave completed! Next wave in {waveCooldown} seconds...");
        yield return new WaitForSeconds(waveCooldown);
        currentWaveSpawnCount = 0;
        isWaveCooldown = false;
        Debug.Log("New wave starting!");
    }
    
    /// <summary>
    /// Vẽ spawn area trong Editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(transform.position.x, spawnPositionY, 0);
        Vector3 size = new Vector3(spawnRangeX * 2, 1f, 0);
        Gizmos.DrawWireCube(center, size);
        
        // Vẽ các vị trí spawn có thể
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector3(-spawnRangeX, spawnPositionY, 0), 0.5f);
        Gizmos.DrawWireSphere(new Vector3(spawnRangeX, spawnPositionY, 0), 0.5f);
    }
}
