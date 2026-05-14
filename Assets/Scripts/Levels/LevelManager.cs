using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public LevelDataContainer levelData;
    public LevelSpawner spawner;
    
    private int currentLevelIndex = 0;
    private float levelTimer;
    private bool isTransitioning = false; 

    void Start()
    {
        StartLevel(0);
    }

    void Update()
    {
        if (isTransitioning) return; 

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
        isTransitioning = false; 
        currentLevelIndex = index;
        levelTimer = 0;
        spawner.UpdateSettings(levelData.levels[index].spawnInterval);
        spawner.SetSpawning(true);
    }

    void EndLevel()
    {
        isTransitioning = true; 
        spawner.SetSpawning(false);
        
        spawner.SpawnObstacle(true); 
        spawner.SpawnObstacle(true); 

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
}