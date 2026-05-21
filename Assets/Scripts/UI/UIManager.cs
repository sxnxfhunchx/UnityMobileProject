    using UnityEngine;
    using TMPro;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        public Image healthFillImage;
        
        [Header("GameOver Settings")]
        public GameObject gameOverPanel; 

        private LevelManager levelManager;
        private PlayerHealth playerHealth;

        void Start()
        {
            levelManager = FindFirstObjectByType<LevelManager>();
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
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
            if (levelManager != null)
            {
                levelText.text = "Level: " + levelManager.GetCurrentLevelNumber();
                float timeLeft = levelManager.GetTimeRemaining();
                timerText.text = "Next Level in: " + (timeLeft > 0 ? timeLeft.ToString("F1") : "0.0") + "s";
            }

            if (GameManager.Instance != null)
            {
                scoreText.text = "Score: " + GameManager.Instance.Score;
            }

           
            if (playerHealth != null && healthFillImage != null)
            {
                float healthPercent = (float)playerHealth.GetCurrentHealth() / playerHealth.maxHealth;
                healthFillImage.fillAmount = healthPercent;
            }
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null && !gameOverPanel.activeSelf)
            {
                gameOverPanel.SetActive(true);
            }
        }
        public void RestartGame()
        {
            Time.timeScale = 1f; 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
    }