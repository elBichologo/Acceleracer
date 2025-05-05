using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;
    
    // Dimensiones de la calle (15x10)
    public float streetWidth = 15f;
    public float streetLength = 10f;
    
    // Altura de spawn para el powerup
    public float spawnY = 1f;
    
    // Tiempo entre spawns (en segundos)
    public float minSpawnTime = 10f;
    public float maxSpawnTime = 30f;
    
    private bool canSpawn = true;
    private Coroutine spawnCoroutine;

    void Start()
    {
        StartSpawning();
    }
    
    IEnumerator SpawnPowerUpRoutine()
    {
        while (canSpawn)
        {
            // Esperar un tiempo aleatorio entre minSpawnTime y maxSpawnTime
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            
            // Verificar si el juego sigue activo
            if (!canSpawn || (GameManager.Instance != null && GameManager.Instance.isGameOver))
                yield break;
                
            SpawnPowerUp();
        }
    }
    
    void SpawnPowerUp()
    {
        // Calcular una posición aleatoria dentro del área del street
        float randomX = Random.Range(-streetWidth/2 + 1f, streetWidth/2 - 1f);
        float randomZ = Random.Range(-streetLength/2 + 1f, streetLength/2 - 1f);
        
        Vector3 spawnPos = new Vector3(randomX, spawnY, randomZ);
        
        Debug.Log($"Spawning power-up at position: {spawnPos}");
        
        Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);
    }
    
    public void StopSpawning()
    {
        canSpawn = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    
    public void StartSpawning()
    {
        canSpawn = true;
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnPowerUpRoutine());
        }
    }
    
    // Para depurar el área de spawn (visible en el editor)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(0, spawnY, 0);
        Vector3 size = new Vector3(streetWidth, 0.1f, streetLength);
        Gizmos.DrawWireCube(center, size);
    }
}