using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    public void SaveGame()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;

        GameSaveRoot save = new GameSaveRoot();

        save.bonusScore = GameManager.Instance.GetBonusScore();
        save.enemiesKilledScore = GameManager.Instance.GetEnemiesKilledScore();
        save.survivalTime = GameManager.Instance.SurvivalTime;

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            save.playerCurrentHealth = playerHealth.CurrentHealth;
            save.playerPosX = playerHealth.transform.position.x;
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            save.currentLevelIndex = levelManager.GetCurrentLevelIndex();
            save.levelTimer = levelManager.GetLevelTimer();
            save.isBossPhaseActive = levelManager.IsBossPhaseActive;
        }

        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
        {
            if (!enemy.gameObject.activeSelf) continue;

            EnemySaveData eData = new EnemySaveData
            {
                poolTag = enemy.GetComponent<EnemyController>().data.poolTag, 
                posX = enemy.transform.position.x,
                posY = enemy.transform.position.y,
                posZ = enemy.transform.position.z,
                health = enemy.GetCurrentHealth()
            };
            save.activeEnemies.Add(eData);
        }

        BonusController[] bonuses = FindObjectsByType<BonusController>(FindObjectsSortMode.None);
        foreach (BonusController bonus in bonuses)
        {
            if (!bonus.gameObject.activeSelf) continue;

            
        }

        string json = JsonUtility.ToJson(save, true);
        
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved:" + saveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("File doesn't exist: " + saveFilePath);
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        
        GameSaveRoot save = JsonUtility.FromJson<GameSaveRoot>(json);

        EnemyController[] currentEnemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in currentEnemies)
        {
            if (enemy.gameObject.activeSelf && ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.ReturnToPool(enemy.GetComponent<EnemyController>().data.poolTag, enemy.gameObject);
            }
        }

        GameManager.Instance.LoadSavedStats(save.bonusScore, save.enemiesKilledScore, save.survivalTime);

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetCurrentHealth(save.playerCurrentHealth);
            Vector3 pPos = playerHealth.transform.position;
            pPos.x = save.playerPosX;
            playerHealth.transform.position = pPos;
            
            PlayerMovement movement = playerHealth.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = true;
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.LoadSavedLevel(save.currentLevelIndex, save.levelTimer, save.isBossPhaseActive);
        }

        if (ObjectPooler.Instance != null)
        {
            foreach (EnemySaveData eData in save.activeEnemies)
            {
                Vector3 spawnPos = new Vector3(eData.posX, eData.posY, eData.posZ);
                GameObject spawnedEnemy = ObjectPooler.Instance.SpawnFromPool(eData.poolTag, spawnPos, Quaternion.Euler(0, 180, 0));
                
                if (spawnedEnemy != null)
                {
                   
                }
            }
        }
    }
}