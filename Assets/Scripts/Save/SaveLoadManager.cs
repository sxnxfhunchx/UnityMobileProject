using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Ability;
using Save;
using SO;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private CharactersDatabase characterDatabase;
    
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

    public event Action OnSaveCompleted;
    
    private string _saveFolderPath;
    private string _timestampFormat = "yyyy-MM-dd_HH-mm-ss";
    
    private byte[] _pendingScreenshot;
    
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

    private void Start()
    {
        _saveFolderPath = Path.Combine(Application.persistentDataPath, $"saves");
    }

    private string GetJsonPath(int slotIndex)
    {
        return Path.Combine(_saveFolderPath, $"save_slot_{slotIndex}.json");
    }
    
    private string GetJsonPath(string fileName)
    {
        return Path.Combine(_saveFolderPath, $"{fileName}.json");
    }

    private string GetScreenshotPath(int slotIndex)
    {
        return Path.Combine(_saveFolderPath, $"save_slot_{slotIndex}.png");
    }
    
    private string GetScreenshotPath(string fileName)
    {
        return Path.Combine(_saveFolderPath, $"{fileName}.png");
    }

    private string CreateFileName(string character, int level, string timestamp)
    {
        string baseName = $"save_L{level:00}_{character}_{timestamp}";
        return baseName;
    }

    private void ParseFileName(string filePath, out string character, out string level, out string timestamp)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        // save_L02_Barbarian_2026-06-27_21-45-13

        string[] parts = fileName.Split('_');

        level = parts[1].Replace("L", "Level ");
        character = parts[2];
        timestamp = parts[3] + " " + parts[4].Replace("-", ":");
    }
    
    public void ExecuteSave()
    {
        StartCoroutine(CaptureAndSaveRoutine());
    }

    private IEnumerator CaptureAndSaveRoutine()
    {
        yield return new WaitForEndOfFrame();
        
        GameplaySaveData saveData = new GameplaySaveData();
        saveData.saveDate = DateTime.Now.ToString(_timestampFormat);

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

        saveData.characterId = GameManager.Instance.CurrentCharacter.characterId;
        
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
        
        string fileName = CreateFileName(GameManager.Instance.CurrentCharacter.characterName,
            levelManager.CurrentLevelSettings.levelNumber, saveData.saveDate);

        EnsureSaveFolderExists();
        File.WriteAllText(GetJsonPath(fileName), jsonString, Encoding.UTF8);
        
        //Texture2D screenTex = new Texture2D(width, height, TextureFormat.RGB24, false);

        if (_pendingScreenshot != null)
        {
            File.WriteAllBytes(GetScreenshotPath(fileName), _pendingScreenshot);
        }
        
        Debug.Log($"Game {fileName} saved with screenshot!");

        OnSaveCompleted?.Invoke();
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

    public void DeleteSaveFile(string fileName)
    {
        string jsonPath = GetJsonPath(fileName);
        string screenshotPath = GetScreenshotPath(fileName);

        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
        }

        if (File.Exists(screenshotPath))
        {
            File.Delete(screenshotPath);
        }
    }
    
    
    private void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save file at {path}");
            return;
        }

        EnsureSaveFolderExists();
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

        CharacterData character = characterDatabase.GetById(saveData.characterId);
        GameManager.Instance.SetSelectedCharacter(character);
        CharacterInitializer characterInitializer = FindFirstObjectByType<CharacterInitializer>();
        characterInitializer.Initialize();
        
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
    
    public void ExecuteLoad(int slotIndex)
    {
        string path = GetJsonPath(slotIndex);
        Load(path);
    }
    
    public void ExecuteLoad(string fileName)
    {
        string path = GetJsonPath(fileName);
        Load(path);
    }
    
    public List<SaveFileData> GetSavedGames()
    {
        List<SaveFileData> saves = new List<SaveFileData>();
        
        if (!Directory.Exists(_saveFolderPath))
            return saves;

        string[] files = Directory.GetFiles(_saveFolderPath, "*.json");

        foreach (string file in files)
        {
            SaveFileData saveData = new SaveFileData();
            
            string fileName = Path.GetFileNameWithoutExtension(file);
            
            ParseFileName(fileName, out string character, out string level, out string timestamp);
            saveData.SaveFileName = fileName;
            saveData.SaveName = $"{level}\n{character}";
            saveData.Date = File.GetLastWriteTime(file);
            
            saveData.ThumbnailPath = Path.ChangeExtension(file, ".png");
            
            saves.Add(saveData);
        }
        
        return saves;
    }

    private Sprite LoadThumbnail(string filePath)
    {
        if (!File.Exists(filePath))
            return null;
        
        byte[] bytes = File.ReadAllBytes(filePath);
        
        Texture2D texture = new Texture2D(2, 2);
        
        if (!texture.LoadImage(bytes))
            return null;
        
        Sprite sprite =  Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        return sprite;
    }
    
    private void EnsureSaveFolderExists()
    {
        Directory.CreateDirectory(_saveFolderPath);
    }

    public void SetPendingScreenshot(byte[] screenshot)
    {
        _pendingScreenshot = screenshot;
    }
}