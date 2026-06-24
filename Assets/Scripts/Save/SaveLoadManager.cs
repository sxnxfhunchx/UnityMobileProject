using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Ability;

public class SaveLoadManager : MonoBehaviour
{

    private static SaveLoadManager _instance;
    public static SaveLoadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SaveLoadManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private string GetJsonPath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slotIndex}.json");
    }

    public string GetScreenshotPath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slotIndex}.png");
    }

    public void ExecuteSave(int slotIndex)
    {
        HUDManager hud = FindFirstObjectByType<HUDManager>();
        StartCoroutine(CaptureAndSaveRoutine(slotIndex, hud));
    }

    private IEnumerator CaptureAndSaveRoutine(int slotIndex, HUDManager hud)
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenTex = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        screenTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenTex.Apply();

        byte[] imageBytes = screenTex.EncodeToPNG();
        Destroy(screenTex);

        File.WriteAllBytes(GetScreenshotPath(slotIndex), imageBytes);

        GameplaySaveData saveData = new GameplaySaveData();
        saveData.saveSlotID = slotIndex.ToString();
        saveData.saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (GameManager.Instance != null)
        {
            saveData.survivalTime = GameManager.Instance.SurvivalTime;
            saveData.enemiesKilledScore = GameManager.Instance.GetEnemiesKilledScore();
            saveData.bonusScore = GameManager.Instance.GetBonusScore();
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            saveData.currentLevelIndex = levelManager.GetCurrentLevelIndex();
            saveData.levelTimer = levelManager.GetLevelTimer();
        }

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            saveData.playerHealth = player.CurrentHealth;
            saveData.playerPosX = player.transform.position.x;
        }

        PlayerAbilityController abilityController = FindFirstObjectByType<PlayerAbilityController>();
        if (abilityController != null && abilityController.CurrentPowerUpData != null)
        {
            saveData.currentAbilityTag = abilityController.CurrentPowerUpData.poolTag;
        }

        EnemyController[] activeEnemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (var enemy in activeEnemies)
        {
            if (!enemy.gameObject.activeSelf || enemy.GetEnemyData() == null) continue;

            ActiveEntitySaveData entityData = new ActiveEntitySaveData
            {
                poolTag = enemy.GetEnemyData().poolTag,
                posX = enemy.transform.position.x,
                posY = enemy.transform.position.y,
                posZ = enemy.transform.position.z,
                currentHealth = enemy.GetCurrentHealth()
            };
            saveData.activeEnemies.Add(entityData);
        }

        string jsonString = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(GetJsonPath(slotIndex), jsonString, Encoding.UTF8);

        Debug.Log($"Slot {slotIndex} saved with screenshot!");

        if (hud != null)
        {
            hud.OnSaveFinished();
        }
    }
    
    public void DeleteAllSaves()
    {
        for (int i = 1; i <= 3; i++)
        {
            string jsonPath = GetJsonPath(i);
            string screenshotPath = GetScreenshotPath(i);

            if (File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
            }

            if (File.Exists(screenshotPath))
            {
                File.Delete(screenshotPath);
            }
        }
    }

    public void ExecuteLoad(int slotIndex)
    {
        string path = GetJsonPath(slotIndex);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save file found for slot {slotIndex}");
            return;
        }

        string jsonString = File.ReadAllText(path, Encoding.UTF8);
        GameplaySaveData saveData = JsonUtility.FromJson<GameplaySaveData>(jsonString);

        EnemyController[] currentEnemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (var enemy in currentEnemies)
        {
            if (enemy.gameObject.activeSelf && ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.ReturnToPool(enemy.GetEnemyData().poolTag, enemy.gameObject);
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestoreSessionStats(saveData.enemiesKilledScore, saveData.bonusScore, saveData.survivalTime);
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.RestoreLevelState(saveData.currentLevelIndex, saveData.levelTimer);
        }

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.RestoreHealth(saveData.playerHealth);
            Vector3 pPos = player.transform.position;
            pPos.x = saveData.playerPosX;
            player.transform.position = pPos;
        }

        if (ObjectPooler.Instance != null)
        {
            TargetProvider targetProvider = FindFirstObjectByType<TargetProvider>();
            LevelManager lvlManager = FindFirstObjectByType<LevelManager>();

            foreach (var entity in saveData.activeEnemies)
            {
                Vector3 spawnPos = new Vector3(entity.posX, entity.posY, entity.posZ);
                GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(entity.poolTag, spawnPos, Quaternion.Euler(0, 180, 0));
                
                if (enemyObj != null && enemyObj.TryGetComponent(out EnemyController controller))
                {
                    EnemyData foundData = null;
                    if (lvlManager != null && lvlManager.CurrentLevelSettings != null)
                    {
                        foreach (var entry in lvlManager.CurrentLevelSettings.spawnSettings.enemies)
                        {
                            if (entry.enemyData != null && entry.enemyData.poolTag == entity.poolTag)
                            {
                                foundData = entry.enemyData;
                                break;
                            }
                        }
                    }

                    if (foundData == null && lvlManager != null && lvlManager.CurrentLevelSettings != null)
                    {
                        foreach (var boss in lvlManager.CurrentLevelSettings.spawnSettings.bosses)
                        {
                            if (boss != null && boss.poolTag == entity.poolTag)
                            {
                                foundData = boss;
                                break;
                            }
                        }
                    }

                    controller.Initialize(foundData, targetProvider);
                    
                    controller.SetSavedHealth(entity.currentHealth);
                    controller.ApplyEnemySettings();
                }
            }
        }
    }
}