using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text scoreText;  // You can leave this or use the UIManager instead
    public float survivalTime = 0f;
    public int collectedPoints = 0;
    public bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isGameOver) return;

        survivalTime += Time.deltaTime;
        UpdateScoreUI();
    }

    public void AddPowerUpPoints(int points)
    {
        collectedPoints += points;
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("Juego terminado. Puntaje final: " + TotalScore());
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + TotalScore();
    }

    public int TotalScore()
    {
        return Mathf.FloorToInt(survivalTime * 3) + collectedPoints;
    }
}
