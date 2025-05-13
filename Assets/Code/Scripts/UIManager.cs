using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    public AudioSource gameOverAudio;  // 🎵 Añadido

    private GameManager gameManager;
    private bool gameOverHandled = false;  // ✅ Para evitar múltiples llamadas

    void Awake()
    {
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

                if (gameManager.isGameOver && !gameOverHandled)
                {
                    gameOverHandled = true;

                    // Muestra el panel de Game Over
                    if (gameOverPanel != null)
                        gameOverPanel.SetActive(true);

                    // Muestra la puntuación final
                    if (finalScoreText != null)
                        finalScoreText.text = "Final Score: " + gameManager.TotalScore();

                    // Reproduce el sonido del Game Over, solo cuando se activa el Game Over
                    if (gameOverAudio != null)
                        gameOverAudio.Play();
                }
            }
        }


    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
