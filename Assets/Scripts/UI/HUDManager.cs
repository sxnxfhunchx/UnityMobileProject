using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    [Header("Health")]
    [SerializeField] private Image healthFillImage;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel; 
    
    [Header("References")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private PlayerHealth playerHealth;
    
    void Start()
    {
        if (gameOverPanel != null) 
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        UpdateUI();

        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive)
        {
            ShowGameOver();
        }
    }

    void UpdateUI()
    {
        UpdateLevelUI();
        UpdateScoreUI();
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null || healthFillImage == null)
            return;
        
        float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        healthFillImage.fillAmount = healthPercent;
    }

    private void UpdateScoreUI()
    {
        if (GameManager.Instance == null)
            return;
        
        scoreText.text = "Score: " + GameManager.Instance.Score;
        
        int minutes = Mathf.FloorToInt(GameManager.Instance.SurvivalTime / 60f);
        int seconds = Mathf.FloorToInt(GameManager.Instance.SurvivalTime % 60f);
        
        timeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void UpdateLevelUI()
    {
        if (levelManager == null)
            return;
        
        levelText.text = "Level: " + levelManager.GetCurrentLevelNumber();
       
        float timeLeft = Mathf.Max(0f, levelManager.GetTimeRemaining());
        timerText.text = "Next Level in: " + (timeLeft > 0 ? timeLeft.ToString("F1") : "0.0") + "s";
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null && !gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void OnSaveButtonPressed()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveGame();
        }
    }

    public void OnLoadButtonPressed()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.LoadGame();
        
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
