using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 1.5f;

    // Ajustado a plano de 15 en X y enemigos con Y = 0.5
    public float[] laneXPositions = new float[] { -6.25f, -3.75f, -1.25f, 1.25f, 3.75f, 6.25f };
    public float spawnZ = 18f;
    public float spawnY = 0.8f;
    private bool canSpawn = true;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (!canSpawn || GameManager.Instance.isGameOver) return;
        
        int laneIndex = Random.Range(0, laneXPositions.Length);
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);

        Vector3 spawnPos = new Vector3(laneXPositions[laneIndex], spawnY, spawnZ);
        
        Debug.Log($"Spawning enemy at position: {spawnPos}");

        Instantiate(enemyPrefabs[enemyIndex], spawnPos, Quaternion.identity);
    }
    public void StopSpawning()
    {
        canSpawn = false;
        CancelInvoke(nameof(SpawnEnemy));
    }
    
    public void StartSpawning()
    {
        canSpawn = true;
        if (!IsInvoking(nameof(SpawnEnemy)))
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }
}



