using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float baseSpeed = 5f; // Velocidad base específica de cada tipo de enemigo
    [HideInInspector] public float currentSpeed; // Velocidad actual con bonificación
    [HideInInspector] public EnemySpawner spawner; // Referencia al spawner

    void OnEnable()
    {
        // Asegurarse que la rotación es correcta al activarse
        transform.rotation = Quaternion.Euler(0, 180, 0);
        
        // Actualizar la velocidad
        UpdateSpeed();
        
        Debug.Log($"Enemigo {name} habilitado con velocidad base {baseSpeed}, velocidad actual {currentSpeed}");
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        // Mover usando la velocidad actual
        transform.Translate(0, 0, currentSpeed * Time.deltaTime);

        // Devolver al pool si sale de la zona visible
        if (transform.position.z < -10f)
        {
            ReturnToPool();
        }
    }
    
    // Método para actualizar la velocidad basándose en el tiempo de juego
    public void UpdateSpeed()
    {
        if (GameManager.Instance != null)
        {
            // Calcular cuántos incrementos de 10 segundos han pasado
            int speedIncrements = Mathf.FloorToInt(GameManager.Instance.survivalTime / 10f);
            
            // Cada incremento aumenta la velocidad en 0.2
            float speedBonus = speedIncrements * 0.2f;
            
            // IMPORTANTE: Aplicar la bonificación a la velocidad base específica de este enemigo
            currentSpeed = baseSpeed + speedBonus;
            
            // Debug para verificar que la velocidad está aumentando
            if (speedIncrements > 0)
                Debug.Log($"Enemigo {name}: velocidad base {baseSpeed}, bonus +{speedBonus}, velocidad actual {currentSpeed}");
        }
        else
        {
            // Si no hay GameManager, usar la velocidad base
            currentSpeed = baseSpeed;
        }
    }
    
    void ReturnToPool()
    {
        if (spawner != null)
        {
            spawner.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}