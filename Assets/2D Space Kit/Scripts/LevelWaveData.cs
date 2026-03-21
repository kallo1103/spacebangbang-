using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int numberOfEnemy;
    public Vector3 formationOffset;
    public int pathIndex;
    public float speed;
    public float nextWaveDelay;
}

[CreateAssetMenu(fileName = "NewLevelWaveData", menuName = "SpaceBangBang/LevelWaveData")]
public class LevelWaveData : ScriptableObject
{
    public EnemyWave[] enemyWaves;
}
