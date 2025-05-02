using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private float score = 0f;
    private bool isAlive = true;

    void Update()
    {
        if (!isAlive) return;

        // Movimiento básico
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Puntaje basado en tiempo
        score += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            GameManager.Instance.GameOver();
            GameOver();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("powerup"))
        {
            score += 10f; // Por ejemplo, 10 puntos por powerup
            Destroy(other.gameObject);
        }
    }

    void GameOver()
    {
        isAlive = false;
        Debug.Log("Game Over. Puntaje final: " + Mathf.RoundToInt(score));
        // Aquí puedes llamar a un manejador de UI o reiniciar escena:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

