using SO;
using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
    [SerializeField] private CharactersDatabase characterDatabase;
    
    private void Start()
    {
        GameplaySaveData pendingSave = GameManager.Instance.ConsumePendingSave();
        
        if (pendingSave != null)
        {
            ApplySave(pendingSave);
        }
        else
        {
            StartNewGame();
        }
    }

    public void ApplySave(GameplaySaveData saveData)
    {
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

    private void StartNewGame()
    {
        // do something here maybe
    }
}
