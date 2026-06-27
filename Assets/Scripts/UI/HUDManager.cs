using Ability;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    
    [Header("Dash")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Button dashButton;

    [Header("Ability")]
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private Button abilityButton;
    
    [Header("Options Menu")]
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private Button optionsMenuButton;

    [Header("Save/Load Menu")]
    [SerializeField] private GameObject saveLoadMenuPanel; 
    [SerializeField] private Button saveLoadMenuButton;
    
    [Header("Save/Load Buttons References")]
    [SerializeField] private TextMeshProUGUI[] saveButtonsTexts; 
    [SerializeField] private TextMeshProUGUI[] loadButtonsTexts; 
    [SerializeField] private Button[] loadButtons;
    private void OnEnable()
    {
        abilityController.OnAbilityAvailabilityChanged += ToggleAbilityAvailability;
        playerMovement.OnDashAvailabilityChanged += ToggleDashAvailability;
        ToggleDashAvailability(true);
    }

    private void OnDisable()
    {
        abilityController.OnAbilityAvailabilityChanged -= ToggleAbilityAvailability;
        playerMovement.OnDashAvailabilityChanged -= ToggleDashAvailability;
    }
    
    void Start()
    {
        Debug.Log("Hud Manager Start");
        if (gameOverPanel != null) 
            gameOverPanel.SetActive(false);
        
        if (saveLoadMenuPanel != null)
            saveLoadMenuPanel.SetActive(false);
        
        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);
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
    
    public void OpenOptionsMenu()
    {
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void CloseOptionsMenu()
    {
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(false);
            if (GameManager.Instance != null && GameManager.Instance.IsGameActive)
            {
                Time.timeScale = 1f;
            }
        }
    }
    
    public void OpenSaveLoadMenu()
    {
        StartCoroutine(OpenSaveMenuRoutine());
    }

    private IEnumerator OpenSaveMenuRoutine()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        byte[] imageBytes = screenshot.EncodeToPNG();
        Destroy(screenshot);

        SaveLoadManager.Instance.SetPendingScreenshot(imageBytes);
        
        if (saveLoadMenuPanel != null)
        {
            saveLoadMenuPanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }

    public void CloseSaveLoadMenu()
    {
        if (saveLoadMenuPanel != null)
        {
            saveLoadMenuPanel.SetActive(false);
            if (GameManager.Instance != null && GameManager.Instance.IsGameActive)
            {
                Time.timeScale = 1f; 
            }
        }
        SaveLoadManager.Instance.SetPendingScreenshot(null);
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
        
        scoreText.text = "SCORE: " + GameManager.Instance.Score;
        
        int minutes = Mathf.FloorToInt(GameManager.Instance.SurvivalTime / 60f);
        int seconds = Mathf.FloorToInt(GameManager.Instance.SurvivalTime % 60f);
        
        timeText.text = $"TIME: {minutes:00}:{seconds:00}";
    }

    private void UpdateLevelUI()
    {
        if (levelManager == null)
            return;
        
        levelText.text = "LVL " + levelManager.GetCurrentLevelNumber();
       
        float timeLeft = Mathf.Max(0f, levelManager.GetTimeRemaining());
        timerText.text = "NEXT IN: " + (timeLeft > 0 ? timeLeft.ToString("F1") : "0.0") + "s";
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
        GameManager.Instance.StartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void ToggleDashAvailability(bool canDash)
    {
        dashButton.interactable = canDash;
    }
    
    private void ToggleAbilityAvailability(bool available)
    {
        abilityButton.interactable = available;
    }
    
    public void ClearAllSaveSlots()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeleteAllSaves();
        }
    }
    
    /*
    public void LoadGame(int slotIndex)
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.ExecuteLoad(slotIndex);
            CloseSaveLoadMenu();
        }
    }
    */
}
