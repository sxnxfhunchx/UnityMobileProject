using System.Collections;
using SO;
using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
    [SerializeField] private CharactersDatabase characterDatabase;
    
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        
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
        if (characterInitializer != null)
        {
            characterInitializer.Initialize();
        }
        
        if (!string.IsNullOrEmpty(saveData.currentAbilityTag))
        {
            Ability.PlayerAbilityController abilityController = FindFirstObjectByType<Ability.PlayerAbilityController>();
            LevelManager lvlManagerRef = FindFirstObjectByType<LevelManager>();

            if (abilityController != null && lvlManagerRef != null && lvlManagerRef.CurrentLevelSettings != null)
            {
                SO.PowerUps.PowerUpData foundPowerUp = null;
                var powerUpsList = lvlManagerRef.CurrentLevelSettings.spawnSettings.powerUps;
            
                if (powerUpsList != null)
                {
                    foreach (var powerUp in powerUpsList)
                    {
                        if (powerUp != null && powerUp.poolTag == saveData.currentAbilityTag)
                        {
                            foundPowerUp = powerUp;
                            break;
                        }
                    }
                }

                if (foundPowerUp != null)
                {
                    abilityController.SetAbility(foundPowerUp);
                }
            }
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

            if (saveData.activeItems != null)
            {
                foreach (var item in saveData.activeItems)
                {
                    Vector3 spawnPos = new Vector3(item.posX, item.posY, item.posZ);
                    
                    GameObject itemObj = ObjectPooler.Instance.SpawnFromPoolWithPrefabRotation(item.poolTag, spawnPos);

                    if (itemObj == null)
                    {
                        itemObj = ObjectPooler.Instance.SpawnFromPool(item.poolTag, spawnPos, Quaternion.identity);
                    }

                    if (itemObj != null)
                    {
                        if (itemObj.TryGetComponent(out PowerUpPickup pickup) && lvlManager != null && lvlManager.CurrentLevelSettings != null)
                        {
                            foreach (var powerUpData in lvlManager.CurrentLevelSettings.spawnSettings.powerUps)
                            {
                                string uniqueTag = string.IsNullOrEmpty(powerUpData.poolTag) ? powerUpData.name : powerUpData.poolTag;
                                if (uniqueTag == item.poolTag)
                                {
                                    pickup.Initialize(powerUpData);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        } 
    }

    private void StartNewGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}
