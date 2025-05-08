using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  // 0: moto, 1: hiperdeportivo, 2: camión, 3: vehículo normal
    
    // Intervalo inicial de spawn (cada 1.5 segundos)
    public float initialSpawnInterval = 1.5f;
    
    // Intervalo final de spawn (cada 0.6 segundos para 2.5x más vehículos)
    public float finalSpawnInterval = 0.15f;
    
    // Intervalo actual (se actualizará dinámicamente)
    private float currentSpawnInterval;
    
    public int poolSizePerType = 10;
    
    // Ajustado a plano de 15 en X
    public float[] laneXPositions = new float[] { -6.25f, -3.75f, -1.25f, 1.25f, 3.75f, 6.25f };
    public float spawnZ = 5f; // Reducido de 18f a 5f
    public float spawnY = 1.2f; // Altura ajustada como solicitaste
    private bool canSpawn = true;
    
    // Tiempo para alcanzar la dificultad máxima (en segundos)
    public float maxDifficultyTime = 120f;
    
    // Probabilidades iniciales (en porcentajes)
    // 0: moto, 1: hiperdeportivo, 2: camión, 3: vehículo normal
    private float[] initialProbabilities = new float[] { 10f, 10f, 30f, 50f };
    
    // Probabilidades finales (ajustadas para más vehículos pequeños)
    private float[] finalProbabilities = new float[] { 55f, 45f, 0f, 0f };
    
    // Pools de enemigos
    private List<GameObject>[] enemyPools;
    private Transform poolContainer;
    
    // Coroutine para actualizar velocidad de enemigos
    private Coroutine speedUpdateCoroutine;
    
    // Coroutine para ajustar intervalo de spawn
    private Coroutine spawnIntervalCoroutine;

    void Awake()
    {
        // Crear un contenedor para organizar la jerarquía
        poolContainer = new GameObject("EnemyPool").transform;
        poolContainer.SetParent(transform);

        // Inicializar los pools
        InitializePools();
        
        // Verificar que las configuraciones de probabilidad son correctas
        ValidateProbabilities();
        
        // Inicializar el intervalo de spawn
        currentSpawnInterval = initialSpawnInterval;
    }
    
    void ValidateProbabilities()
    {
        // Verificar que hay tantas probabilidades como tipos de enemigos
        if (initialProbabilities.Length != enemyPrefabs.Length || 
            finalProbabilities.Length != enemyPrefabs.Length)
        {
            Debug.LogError("Las probabilidades no coinciden con la cantidad de enemigos");
        }
        
        // Verificar que las probabilidades suman 100%
        float initialSum = 0, finalSum = 0;
        for (int i = 0; i < initialProbabilities.Length; i++)
        {
            initialSum += initialProbabilities[i];
            finalSum += finalProbabilities[i];
        }
        
        if (Mathf.Abs(initialSum - 100f) > 0.01f || Mathf.Abs(finalSum - 100f) > 0.01f)
        {
            Debug.LogWarning($"Las probabilidades no suman 100%: Inicial={initialSum}, Final={finalSum}");
        }
    }

    void InitializePools()
    {
        enemyPools = new List<GameObject>[enemyPrefabs.Length];

        // Para cada tipo de enemigo
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            enemyPools[i] = new List<GameObject>();
            
            // Crear un contenedor para cada tipo
            Transform typeContainer = new GameObject($"Pool_{enemyPrefabs[i].name}").transform;
            typeContainer.SetParent(poolContainer);
            
            // Crear la cantidad especificada de cada tipo
            for (int j = 0; j < poolSizePerType; j++)
            {
                GameObject enemy = Instantiate(enemyPrefabs[i], Vector3.zero, Quaternion.identity);
                enemy.name = $"{enemyPrefabs[i].name}_{j}";
                enemy.SetActive(false);
                enemy.transform.SetParent(typeContainer);
                
                // Modificar el comportamiento del enemigo para usar el pool
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.spawner = this;
                }
                
                // OPCIONAL: Si realmente necesitas un Rigidbody
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }
                
                enemyPools[i].Add(enemy);
            }
        }
    }

    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Default"), false);
        // Iniciar el temporizador para actualizar la velocidad cada 10 segundos
        speedUpdateCoroutine = StartCoroutine(UpdateEnemySpeedRoutine());
        
        // Iniciar el temporizador para ajustar el intervalo de spawn
        spawnIntervalCoroutine = StartCoroutine(AdjustSpawnIntervalRoutine());
        
        // Iniciar el spawn de enemigos
        InvokeRepeating(nameof(SpawnEnemy), 1f, currentSpawnInterval);
    }
    
    // Coroutine para actualizar la velocidad de todos los enemigos activos cada 10 segundos
    IEnumerator UpdateEnemySpeedRoutine()
    {
        Debug.Log("Iniciando rutina de actualización de velocidad de enemigos");
        
        while (canSpawn)
        {
            yield return new WaitForSeconds(10f);
            
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager.Instance es nulo. No se puede actualizar velocidad.");
                continue;
            }
            
            if (GameManager.Instance.isGameOver)
                continue;
                
            Debug.Log($"Actualizando velocidad en t={GameManager.Instance.survivalTime}s");
                
            // Actualizar la velocidad de todos los enemigos activos
            UpdateAllActiveEnemiesSpeed();
            
            // Y todos los inactivos también para que estén listos cuando se activen
            UpdateAllInactiveEnemiesSpeed();
            
            int speedIncrements = Mathf.FloorToInt(GameManager.Instance.survivalTime / 10f);
            float speedBonus = speedIncrements * 0.2f;
            Debug.Log($"Nueva bonificación de velocidad: +{speedBonus} ({speedIncrements} incrementos)");
        }
    }
    
    // Actualiza la velocidad de todos los enemigos activos
    void UpdateAllActiveEnemiesSpeed()
    {
        foreach (var pool in enemyPools)
        {
            foreach (var enemy in pool)
            {
                if (enemy.activeInHierarchy)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.UpdateSpeed();
                    }
                }
            }
        }
    }
    
    // Actualiza la velocidad de todos los enemigos inactivos
    void UpdateAllInactiveEnemiesSpeed()
    {
        foreach (var pool in enemyPools)
        {
            foreach (var enemy in pool)
            {
                if (!enemy.activeInHierarchy)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.UpdateSpeed();
                    }
                }
            }
        }
    }
    
    // Coroutine para ajustar gradualmente el intervalo de spawn
    IEnumerator AdjustSpawnIntervalRoutine()
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(1f); // Verificar cada segundo
            
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager.Instance es nulo. No se puede ajustar intervalo de spawn.");
                continue;
            }
            
            if (!GameManager.Instance.isGameOver)
            {
                // Calcular el factor de progresión
                float progressFactor = Mathf.Clamp01(GameManager.Instance.survivalTime / maxDifficultyTime);
                
                // Interpolar entre el intervalo inicial y final
                float newInterval = Mathf.Lerp(initialSpawnInterval, finalSpawnInterval, progressFactor);
                
                // Si el intervalo cambió significativamente, actualizar el spawn
                if (Mathf.Abs(newInterval - currentSpawnInterval) > 0.1f)
                {
                    currentSpawnInterval = newInterval;
                    
                    // Reiniciar el InvokeRepeating con el nuevo intervalo
                    CancelInvoke(nameof(SpawnEnemy));
                    InvokeRepeating(nameof(SpawnEnemy), 0f, currentSpawnInterval);
                    
                    Debug.Log($"Intervalo de spawn actualizado a {currentSpawnInterval:F2}s en t={GameManager.Instance.survivalTime:F1}s");
                }
            }
        }
    }

    void SpawnEnemy()
    {
        if (!canSpawn || (GameManager.Instance != null && GameManager.Instance.isGameOver)) 
            return;
        
        // Seleccionar carril aleatorio
        int laneIndex = Random.Range(0, laneXPositions.Length);
        
        // Seleccionar tipo de enemigo basado en probabilidades dinámicas
        int enemyIndex = GetDynamicEnemyType();

        // Obtener un enemigo del pool
        GameObject enemy = GetEnemyFromPool(enemyIndex);
        if (enemy != null)
        {
            // Posicionar y activar
            Vector3 spawnPos = new Vector3(laneXPositions[laneIndex], spawnY, spawnZ);
            enemy.transform.position = spawnPos;
            
            // Asegurarse que la rotación es correcta
            enemy.transform.rotation = Quaternion.Euler(0, 180, 0);
            
            // Actualizar la velocidad antes de activar
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.UpdateSpeed();
            }
            
            // Activar el enemigo
            enemy.SetActive(true);
            
            // Ignorar colisiones entre este enemigo y otros
            IgnoreEnemyCollisions(enemy);
        }
    }

    // Método auxiliar para ignorar colisiones temporalmente entre enemigos
    void IgnoreEnemyCollisions(GameObject newEnemy)
    {
        Collider newCollider = newEnemy.GetComponent<Collider>();
        if (newCollider == null) return;
        
        // Buscar todos los enemigos activos y configurar Physics.IgnoreCollision
        foreach (var pool in enemyPools)
        {
            foreach (var existingEnemy in pool)
            {
                if (existingEnemy.activeInHierarchy && existingEnemy != newEnemy)
                {
                    Collider existingCollider = existingEnemy.GetComponent<Collider>();
                    if (existingCollider != null)
                    {
                        Physics.IgnoreCollision(newCollider, existingCollider, true);
                    }
                }
            }
        }
    }
    
    // Método para seleccionar un tipo de enemigo según probabilidades dinámicas
    int GetDynamicEnemyType()
    {
        // Calcular el factor de progresión basado en el tiempo de juego
        float gameTime = GameManager.Instance != null ? GameManager.Instance.survivalTime : 0f;
        float progressFactor = Mathf.Clamp01(gameTime / maxDifficultyTime);
        
        // Calcular las probabilidades actuales interpolando entre inicial y final
        float[] currentProbabilities = new float[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            currentProbabilities[i] = Mathf.Lerp(
                initialProbabilities[i], 
                finalProbabilities[i], 
                progressFactor
            );
        }
        
        // Seleccionar un tipo basado en las probabilidades actuales
        float randomValue = Random.Range(0f, 100f);
        float cumulativeProbability = 0f;
        
        for (int i = 0; i < currentProbabilities.Length; i++)
        {
            cumulativeProbability += currentProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return i;
            }
        }
        
        // Por si acaso, retornar el último tipo
        return enemyPrefabs.Length - 1;
    }

    GameObject GetEnemyFromPool(int enemyIndex)
    {
        foreach (GameObject enemy in enemyPools[enemyIndex])
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }
        
        Debug.LogWarning($"Pool de {enemyPrefabs[enemyIndex].name} agotado. Considere aumentar el tamaño del pool.");
        return null;
    }

    public void ReturnToPool(GameObject enemy)
    {
        if (enemy != null)
        {
            enemy.SetActive(false);
        }
    }

    public void StopSpawning()
    {
        canSpawn = false;
        CancelInvoke(nameof(SpawnEnemy));
        
        if (speedUpdateCoroutine != null)
            StopCoroutine(speedUpdateCoroutine);
            
        if (spawnIntervalCoroutine != null)
            StopCoroutine(spawnIntervalCoroutine);
    }
    
    public void StartSpawning()
    {
        canSpawn = true;
        
        if (speedUpdateCoroutine == null)
            speedUpdateCoroutine = StartCoroutine(UpdateEnemySpeedRoutine());
            
        if (spawnIntervalCoroutine == null)
            spawnIntervalCoroutine = StartCoroutine(AdjustSpawnIntervalRoutine());
            
        if (!IsInvoking(nameof(SpawnEnemy)))
            InvokeRepeating(nameof(SpawnEnemy), 0f, currentSpawnInterval);
    }
    
    // Opcional: método para depurar la progresión de dificultad
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            float gameTime = GameManager.Instance != null ? GameManager.Instance.survivalTime : 0;
            float progressFactor = Mathf.Clamp01(gameTime / maxDifficultyTime);
            
            // Calcular velocidad actual de los enemigos
            int speedIncrements = Mathf.FloorToInt(gameTime / 10f);
            float speedBonus = speedIncrements * 0.2f;
            
            // Calcular apariciones por minuto
            float spawnPerMinute = 60f / currentSpawnInterval;
            float initialSpawnPerMinute = 60f / initialSpawnInterval;
            float spawnRatio = spawnPerMinute / initialSpawnPerMinute;
            
            GUILayout.BeginArea(new Rect(10, 40, 300, 220));
            GUILayout.Label($"Tiempo: {gameTime:F1}s / {maxDifficultyTime}s ({progressFactor:P0})");
            GUILayout.Label($"Intervalo spawn: {currentSpawnInterval:F2}s");
            GUILayout.Label($"Enemigos por minuto: {spawnPerMinute:F1} ({spawnRatio:F1}x inicial)");
            GUILayout.Label($"Velocidad enemigos: +{speedBonus:F1} ({speedIncrements} incrementos)");
            
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                float currentProb = Mathf.Lerp(initialProbabilities[i], finalProbabilities[i], progressFactor);
                GUILayout.Label($"{enemyPrefabs[i].name}: {currentProb:F1}%");
            }
            
            GUILayout.EndArea();
        }
    }

    
    // Limpiar al destruir el objeto
    void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
}