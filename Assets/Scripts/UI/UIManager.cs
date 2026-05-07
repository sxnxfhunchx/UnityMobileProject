using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    private LevelManager levelManager;

    void Start()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (levelManager != null)
        {
            levelText.text = "Level: " + levelManager.GetCurrentLevelNumber();

            float timeLeft = levelManager.GetTimeRemaining();
            
            if (timeLeft < 0) timeLeft = 0;
            
            timerText.text = "Next Level in: " + timeLeft.ToString("F1") + "s";
        }

        if (GameManager.Instance != null)
        {
            // Update Score
            scoreText.text = "Score: " + GameManager.Instance.Score;
        }
    }
}