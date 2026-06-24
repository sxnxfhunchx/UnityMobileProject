using UnityEngine;
using Interfaces;


public class LevelManager : MonoBehaviour, ILevelProvider
{
    [SerializeField] private LevelDataContainer levelData;
    
    private int currentLevelIndex = 0;
    private float levelTimer;
    private bool isBossPhaseActive = false;
    
    public int GetCurrentLevelIndex() => currentLevelIndex;
    public float GetLevelTimer() => levelTimer;

    public void RestoreLevelState(int index, float timer)
    {
        currentLevelIndex = index;
        levelTimer = timer;
        isBossPhaseActive = false;
    }

    public LevelSettings CurrentLevelSettings
    {
        get
        {
            if (currentLevelIndex < levelData.levels.Count)
                return levelData.levels[currentLevelIndex];
            
            return null;
        }
    }

    public bool IsRegularEnemyPhaseActive => !isBossPhaseActive;

    public bool IsBossPhaseActive => isBossPhaseActive;
    
    void Start()
    {
        if (levelData == null || levelData.levels.Count == 0)
        {
            Debug.LogError("Level data is missing or empty");
            enabled = false;
            return;
        }
        
        StartLevel(0);
    }

    void Update()
    {
        if (isBossPhaseActive) 
            return; 

        levelTimer += Time.deltaTime;

        if (currentLevelIndex < levelData.levels.Count)
        {
            if (levelTimer >= levelData.levels[currentLevelIndex].levelDuration)
            {
                EndLevel();
            }
        }
    }

    void StartLevel(int index)
    {
        currentLevelIndex = index;
        levelTimer = 0;
        isBossPhaseActive = false;
    }

    void EndLevel()
    {
        isBossPhaseActive = true;
        
        if (currentLevelIndex < levelData.levels.Count - 1)
        {
            Invoke("NextLevel", 3f); 
        }
        else
        {
            Invoke("RepeatLastLevel", 3f);
        }
    }
    
    public int GetCurrentLevelNumber()
    {
        return currentLevelIndex + 1;
    }

    public float GetTimeRemaining()
    {
        if (currentLevelIndex < levelData.levels.Count)
        {
            return levelData.levels[currentLevelIndex].levelDuration - levelTimer;
        }
        return 0;
    }

    void NextLevel() => StartLevel(currentLevelIndex + 1);
    
    private void RepeatLastLevel()
    {
        StartLevel(currentLevelIndex);
    }
    
}