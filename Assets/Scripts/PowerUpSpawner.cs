using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public int poolSize = 2; // Solo necesitamos un par de objetos (por si acaso)
    
    // Dimensiones de la calle (15x10)
    public float streetWidth = 15f;
    public float streetLength = 10f;
    
    // Altura de spawn para el powerup
    public float spawnY = 1f;
    
    // Tiempo entre spawns (en segundos)
    public float minSpawnTime = 10f;
    public float maxSpawnTime = 30f;
    
    // Tiempo de vida del PowerUp
    public float powerUpLifetime = 7f;
    
    private bool canSpawn = true;
    private Coroutine spawnCoroutine;
    private List<GameObject> powerUpPool = new List<GameObject>();
    private Transform poolContainer;

    void Awake()
    {
        // Crear contenedor para el pool
        poolContainer = new GameObject("PowerUpPool").transform;
        poolContainer.SetParent(transform);
        
        // Inicializar el pool
        InitializePool();
    }
    
    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject powerUp = Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity);
            powerUp.name = $"PowerUp_{i}";
            powerUp.SetActive(false);
            powerUp.transform.SetParent(poolContainer);
            
            // Asignar referencia al spawner
            PowerUp powerUpComponent = powerUp.GetComponent<PowerUp>();
            if (powerUpComponent != null)
            {
                powerUpComponent.spawner = this;
                powerUpComponent.lifetime = powerUpLifetime;
            }
            
            powerUpPool.Add(powerUp);
        }
    }

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
        // Obtener un PowerUp del pool
        GameObject powerUp = GetPowerUpFromPool();
        if (powerUp != null)
        {
            // Calcular una posición aleatoria dentro del área del street
            float randomX = Random.Range(-streetWidth/2 + 1f, streetWidth/2 - 1f);
            float randomZ = Random.Range(-streetLength/2 + 1f, streetLength/2 - 1f);
            
            Vector3 spawnPos = new Vector3(randomX, spawnY, randomZ);
            
            // Posicionar y activar
            powerUp.transform.position = spawnPos;
            powerUp.SetActive(true);
            
            // Reiniciar el temporizador del PowerUp
            PowerUp powerUpComponent = powerUp.GetComponent<PowerUp>();
            if (powerUpComponent != null)
            {
                powerUpComponent.ResetLifetime();
            }
            
            Debug.Log($"Spawning power-up from pool at position: {spawnPos}");
        }
    }
    
    GameObject GetPowerUpFromPool()
    {
        // Buscar un PowerUp inactivo en el pool
        foreach (GameObject powerUp in powerUpPool)
        {
            if (!powerUp.activeInHierarchy)
            {
                return powerUp;
            }
        }
        
        Debug.LogWarning("Pool de PowerUp agotado. Todos los PowerUps están activos.");
        return null;
    }
    
    public void ReturnToPool(GameObject powerUp)
    {
        powerUp.SetActive(false);
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