using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private bool isAlive = true;

    void Update()
    {
        if (!isAlive) return;

        // Movimiento básico
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
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
        // Verificar trigger con enemigos por tag
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Trigger con enemigo (tag) detectado");
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
            GameOver();
        }
        // Verificar trigger con enemigos por layer
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Trigger con enemigo (layer) detectado");
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
}