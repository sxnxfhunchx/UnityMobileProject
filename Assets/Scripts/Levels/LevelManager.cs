using UnityEngine;
using Interfaces;


public class LevelManager : MonoBehaviour, ILevelProvider
{
    public LevelDataContainer levelData;
    
    private int currentLevelIndex = 0;
    private float levelTimer;
    
    private bool isBossPhaseActive = false;

    void Start()
    {
        StartLevel(0);
    }

    void Update()
    {
        if (isBossPhaseActive) return; 

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

        currentLevelIndex++;
        
        if (currentLevelIndex < levelData.levels.Count)
        {
            Invoke("NextLevel", 3f); 
        }
        else
        {
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

    void NextLevel() => StartLevel(currentLevelIndex);

    public LevelSettings CurrentLevelSettings
    {
        get
        {
            return levelData.levels[currentLevelIndex];
        }
    }

    public bool IsRegularEnemyPhaseActive
    {
        get
        {
            return !isBossPhaseActive;
        }
    }

    public bool IsBossPhaseActive
    {
        get
        {
            return isBossPhaseActive;
        }
    }
}