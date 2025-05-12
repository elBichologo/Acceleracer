using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private bool isAlive = true;
    
    void Start()
    {
        // Ajustar la escala del modelo
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        
        // Rotar el modelo 180 grados en Y
        transform.rotation = Quaternion.Euler(0, 180, 0);
            
        // Eliminar el BoxCollider anterior si existe
        BoxCollider existingCollider = GetComponent<BoxCollider>();
        if (existingCollider != null)
            Destroy(existingCollider);
                
        // Crear un nuevo BoxCollider con dimensiones ajustadas al tamaño real
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 0.25f, 0); // Centro ajustado a la mitad de la altura
        boxCollider.size = new Vector3(0.8f, 0.5f, 1.5f); // Tamaño real del vehículo
        boxCollider.isTrigger = false;
        
        // Asegurarse que tiene el tag correcto para colisiones
        if (gameObject.tag != "Player")
            gameObject.tag = "Player";
    }

    void Update()
    {
        if (!isAlive) return;

        // Movimiento corregido para rotación de 180 grados
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        // Con rotación de 180 grados, el movimiento debe usar los valores sin invertir
        Vector3 movement = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    // Detección de colisiones físicas (no-trigger)
    void OnCollisionEnter(Collision collision)
    {
        // Verificar colisión con enemigos por tag
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Colisión con enemigo (tag) detectada");
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
            GameOver();
        }
        // Verificar colisión con enemigos por layer
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Colisión con enemigo (layer) detectada");
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
            GameOver();
        }
    }

    // Detección de triggers
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Player colisionó con: {other.gameObject.name}, tag: {other.gameObject.tag}, layer: {other.gameObject.layer}");
        
        // Verificar trigger con enemigos
        if (other.CompareTag("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log($"¡Colisión con enemigo detectada! Nombre: {other.gameObject.name}");
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
            GameOver();
        }
        // Verificar PowerUp
        else if (other.CompareTag("PowerUp"))
        {
            // Obtener referencia al script PowerUp para conocer su valor de puntos
            PowerUp powerUp = other.GetComponent<PowerUp>();
            
            if (powerUp != null)
            {
                // Usar el valor de puntos específico de este power-up
                int points = powerUp.pointsValue;
                
                // Verificar que el GameManager existe
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddPowerUpPoints(points);
                }
                
                // Devolver al pool en lugar de destruir
                if (powerUp.spawner != null)
                {
                    powerUp.spawner.ReturnToPool(other.gameObject);
                }
                else
                {
                    // Fallback si no hay spawner
                    other.gameObject.SetActive(false);
                }
                
                Debug.Log($"PowerUp recogido! +{points} puntos");
            }
        }
    }

    void GameOver()
    {
        isAlive = false;
        Debug.Log("Game Over. Puntaje final: " + GameManager.Instance.TotalScore());
    }
    
    // Método para visualizar el collider (Debug)
    void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.color = Color.green;
            // Dibujar el box collider en la posición correcta
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}