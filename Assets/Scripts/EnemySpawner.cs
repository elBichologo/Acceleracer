using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 1.5f;

    // Ajustado a plano de 20 en X y enemigos con Y = 0.5
    public float[] laneXPositions = new float[] { -7.5f, -4.5f, -1.5f, 1.5f, 4.5f, 7.5f };
    public float spawnZ = 18f;
    public float spawnY = 0.8f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        int laneIndex = Random.Range(0, laneXPositions.Length);
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);

        Vector3 spawnPos = new Vector3(laneXPositions[laneIndex], spawnY, spawnZ);
        Instantiate(enemyPrefabs[enemyIndex], spawnPos, Quaternion.identity);
    }
}



