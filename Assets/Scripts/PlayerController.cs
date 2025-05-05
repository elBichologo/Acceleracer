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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.GameOver();
            GameOver();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            // Obtener referencia al script PowerUp para conocer su valor de puntos
            PowerUp powerUp = other.GetComponent<PowerUp>();
            
            if (powerUp != null)
            {
                // Usar el valor de puntos específico de este power-up
                int points = powerUp.pointsValue;
                GameManager.Instance.AddPowerUpPoints(points);
                // Destruir el objeto power-up
                Destroy(other.gameObject);
                
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
