using Ability;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
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
        if (gameOverPanel != null) 
            gameOverPanel.SetActive(false);
        
        if (saveLoadMenuPanel != null)
            saveLoadMenuPanel.SetActive(false);
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
        if (saveLoadMenuPanel != null)
        {
            saveLoadMenuPanel.SetActive(true);
            
            UpdateSlotLabels();
            
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
    }
    
    private void UpdateSlotLabels()
    {
        for (int i = 1; i <= 3; i++)
        {
            string path = Path.Combine(Application.persistentDataPath, $"save_slot_{i}.json");
            int arrayIndex = i - 1; 

            if (File.Exists(path))
            {
                System.DateTime lastWriteTime = File.GetLastWriteTime(path);
                string formattedDate = lastWriteTime.ToString("dd.MM.yyyy HH:mm");

                saveButtonsTexts[arrayIndex].text = $"Save {i}\n({formattedDate})";
                loadButtonsTexts[arrayIndex].text = $"Load Save {i}\n({formattedDate})";
                
                loadButtons[arrayIndex].interactable = true;
            }
            else
            {
                saveButtonsTexts[arrayIndex].text = "Empty Slot";
                loadButtonsTexts[arrayIndex].text = "No Save Data";
                
                loadButtons[arrayIndex].interactable = false;
            }
        }
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
    
    public void SaveGame(int slotIndex)
    {
        Debug.LogWarning($"[HUDManager] open save with {slotIndex}");
        
        Time.timeScale = 1f;
        
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.ExecuteSave(slotIndex);
        }
    }

    public void OnSaveFinished()
    {
        Time.timeScale = 0f;
        
        UpdateSlotLabels();
    }

    public void LoadGame(int slotIndex)
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.ExecuteLoad(slotIndex);
            CloseSaveLoadMenu();
        }
    }
    
    public void ClearAllSaveSlots()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeleteAllSaves();
            
            UpdateSlotLabels();
        }
    }
        
    [Header("Save/Load Slots UI")]
    [SerializeField] private Image[] slotThumbnails;
}
