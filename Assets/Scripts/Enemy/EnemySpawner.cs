using System;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private MonoBehaviour levelProviderSource;
    
    [Header("Tags")]
    [SerializeField] string[] enemyPoolTags; 
    [SerializeField] string bossPoolTag; 
    
    private ILevelProvider levelProvider;
    private bool bossSpawnedForCurrentLevel = false;
    private float enemySpawnTimer = 0;

    private void Awake()
    {
        levelProvider = levelProviderSource as ILevelProvider;

        if (levelProvider == null)
            Debug.LogError("Level Spawn Provider must implement ILevelSpawnProvider");
    }
    
    private void Update()
    {
        if (levelProvider == null)
            return;

        LevelSettings settings = levelProvider.CurrentLevelSettings;
        
        if (levelProvider.IsRegularEnemyPhaseActive)
        {
            enemySpawnTimer += Time.deltaTime;
            
            if (enemySpawnTimer < settings.spawnInterval)
                return;
            
            enemySpawnTimer = 0f;
            
            SpawnRegularEnemy();
            bossSpawnedForCurrentLevel = false;
        }

        if (levelProvider.IsBossPhaseActive && !bossSpawnedForCurrentLevel)
        {
            SpawnBosses(settings);
            bossSpawnedForCurrentLevel = true;
        }
    }

    private void SpawnRegularEnemy()
    {
        if (enemyPoolTags.Length == 0) return;
        string tag = enemyPoolTags[Random.Range(0, enemyPoolTags.Length)];
        
        SpawnEnemy(tag);
    }

    private void SpawnBosses(LevelSettings settings)
    {
        // TODO: add number of bosses to level settings
        for (int i = 0; i < settings.levelNumber; i++)
        {
            SpawnEnemy(bossPoolTag);
        }
    }
    
    private void SpawnEnemy(string tag)
    {
        Debug.Log($"Spawnin {tag}");
        
        Vector3 spawnPos = GetSpawnPosition();
        
        if (string.IsNullOrEmpty(tag))
            return;

        if (ObjectPooler.Instance == null)
            return;
        
        Debug.Log($"Asking pooler for {tag}");
        ObjectPooler.Instance.SpawnFromPool(tag, spawnPos, Quaternion.Euler(0, 180, 0));
    }
    
    private Vector3 GetSpawnPosition()
    {
        float randomX = Random.Range(-5f, 5f);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y, transform.position.z);

        return spawnPos;
    }
    
    private void OnValidate()
    {
        if (levelProviderSource != null && levelProviderSource is not ILevelProvider)
        {
            Debug.LogWarning("Level provider must implement ILevelProvider");
            levelProviderSource = null;
        }
    }
}
