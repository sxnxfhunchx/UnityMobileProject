using SO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private CharactersDatabase characterDatabase;
    
    private int bonusScore;
    private int enemiesKilledScore;
    
    private float questTimeAccumulator;
    
    public int Score =>  Mathf.FloorToInt(SurvivalTime) + enemiesKilledScore + bonusScore;
    
    public bool IsGameActive { get; private set; }
    public float SurvivalTime { get; private set; }
    
    public int GetEnemiesKilledScore() => enemiesKilledScore;
    public int GetBonusScore() => bonusScore;
    
    public CharacterData SelectedCharacter { get; private set; }

    public CharacterData CurrentCharacter => SelectedCharacter != null ? SelectedCharacter : characterDatabase.GetDefault();
    
    public GameplaySaveData PendingSave { get; private set; }
    
    public void RestoreSessionStats(int savedEnemiesScore, int savedBonus, float savedSurvivalTime)
    {
        enemiesKilledScore = savedEnemiesScore;
        bonusScore = savedBonus;
        SurvivalTime = savedSurvivalTime;
        IsGameActive = true;
        Time.timeScale = 1f;
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        IsGameActive = false;
    }

    private void Update()
    {
        if (!IsGameActive)
            return;
        
        SurvivalTime +=  Time.deltaTime;
        
        questTimeAccumulator += Time.deltaTime;
        if (questTimeAccumulator >= 1f)
        {
            int secondsPassed = Mathf.FloorToInt(questTimeAccumulator);
            questTimeAccumulator -= secondsPassed;

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.TrackProgress(QuestType.SurviveTime, secondsPassed);
            }
        }
    }
    
    public void SetPendingSave(GameplaySaveData saveData)
    {
        PendingSave = saveData;
    }

    public GameplaySaveData ConsumePendingSave()
    {
        GameplaySaveData save = PendingSave;
        PendingSave = null;
        return save;
    }

    public void AddEnemyKillScore(int value)
    {
        enemiesKilledScore += value;
    }
    
    public void AddBonusScore(int value)
    {
        bonusScore += value;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        IsGameActive = true;
        SurvivalTime = 0f;
        enemiesKilledScore = 0;
        bonusScore = 0;
        questTimeAccumulator = 0f;
    }

    public void GameOver()
    {
        IsGameActive = false;
        Time.timeScale = 0f; 
    }
    
    // TODO: maybe remove it to a separate class?
    public float GetDifficultyPowerUpMultiplier()
    {
        int difficulty = PlayerPrefs.GetInt("GameDifficulty", 2);
        float difficultyMultiplier = 1f;

        if (difficulty == 1) difficultyMultiplier = 0.8f; 
        if (difficulty == 3) difficultyMultiplier = 1.25f; 
        
        return difficultyMultiplier;
    }
    
    public float GetDifficultyEnemySpeedMultiplier()
    {
        int difficulty = PlayerPrefs.GetInt("GameDifficulty", 2);
        float difficultyMultiplier = 1f;

        if (difficulty == 1) difficultyMultiplier = 0.75f; 
        if (difficulty == 3) difficultyMultiplier = 1.35f; 
        
        return difficultyMultiplier;
    }
    
    public float GetDifficultyEnemyDamageMultiplier()
    {
        int difficulty = PlayerPrefs.GetInt("GameDifficulty", 2);
        float difficultyMultiplier = 1f;

        if (difficulty == 1) difficultyMultiplier = 0.8f; 
        if (difficulty == 3) difficultyMultiplier = 1.25f; 
        
        return difficultyMultiplier;
    }
    
    public float GetDifficultyBonusChanceMultiplier()
    {
        int difficulty = PlayerPrefs.GetInt("GameDifficulty", 2);
        float difficultyMultiplier = 1f;

        if (difficulty == 1) difficultyMultiplier = 0.8f; 
        if (difficulty == 3) difficultyMultiplier = 1.25f; 
        
        return difficultyMultiplier;
    }
    
    public void SetSelectedCharacter(CharacterData data)
    {
        SelectedCharacter = data;
    }
    
    
}