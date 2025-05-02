using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text scoreText;
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
        scoreText.text = "Puntaje: " + TotalScore();
    }

    int TotalScore()
    {
        return Mathf.FloorToInt(survivalTime) + collectedPoints;
    }
}
