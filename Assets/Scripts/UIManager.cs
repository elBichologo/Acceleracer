using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;  // Nuevo: referencia al texto del puntaje final

    private GameManager gameManager;

    void Awake()
    {
        // Asegúrate que el panel esté desactivado al inicio
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (gameManager != null)
        {
            if (scoreText != null)
                scoreText.text = "Score: " + gameManager.TotalScore();
            
            if (timeText != null)
                timeText.text = "Time: " + Mathf.FloorToInt(gameManager.survivalTime) + "s";

            // Si el juego termina, mostrar panel y actualizar puntaje final
            if (gameManager.isGameOver && gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (finalScoreText != null)
                    finalScoreText.text = "Final Score: " + gameManager.TotalScore();
            }
        }
    }

    // Método que se llamará desde el botón Restart
    public void RestartGame()
    {
        // Recarga la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}