using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private MonoBehaviour levelProviderSource;
    [SerializeField] private GameObject targetProviderSource;

    private ITargetProvider targetProvider;
    private ILevelProvider levelProvider;
    private bool bossSpawnedForCurrentLevel = false;
    private float enemySpawnTimer = 0;

    private void Awake()
    {
        levelProvider = levelProviderSource as ILevelProvider;
        if (levelProvider == null)
            Debug.LogError("Level Spawn Provider must implement ILevelProvider");
        
        targetProvider = targetProviderSource.GetComponent<ITargetProvider>();
        if (targetProvider == null)
            Debug.LogError("Target provider must implement ITargetProvider");
    }
    
    private void Update()
    {
        if (levelProvider == null)
            return;

        LevelSettings settings = levelProvider.CurrentLevelSettings;
        
        if (settings == null)
            return;
        
        if (levelProvider.IsRegularEnemyPhaseActive)
        {
            HandleRegularEnemySpawning(settings.spawnSettings);
            bossSpawnedForCurrentLevel = false;
        }

        if (levelProvider.IsBossPhaseActive && !bossSpawnedForCurrentLevel)
        {
            SpawnBosses(settings.spawnSettings);
            bossSpawnedForCurrentLevel = true;
        }
    }

    private void HandleRegularEnemySpawning(SpawnSettings spawnSettings)
    {
        enemySpawnTimer += Time.deltaTime;
            
        if (enemySpawnTimer < spawnSettings.spawnInterval)
            return;
            
        enemySpawnTimer = 0f;
            
        SpawnRegularEnemy(spawnSettings);
    }

    private void SpawnRegularEnemy(SpawnSettings spawnSettings)
    {
        EnemyData enemyData = spawnSettings.GetRandomEnemy();
        
        if (enemyData == null) 
            return;

        SpawnEnemy(enemyData, spawnSettings);
    }

    private void SpawnBosses(SpawnSettings spawnSettings)
    {
        for (int i = 0; i < spawnSettings.bossesCount; i++)
        {
            EnemyData enemyData = spawnSettings.GetRandomBoss();
            SpawnEnemy(enemyData, spawnSettings);
        }
    }
    
    private void SpawnEnemy(EnemyData enemyData, SpawnSettings spawnSettings)
    {
        if (enemyData == null || string.IsNullOrEmpty(enemyData.poolTag))
            return;

        if (ObjectPooler.Instance == null)
            return;
    
        Vector3 spawnPos = GetSpawnPosition(spawnSettings);
    
        GameObject spawnedObj = ObjectPooler.Instance.SpawnFromPool(enemyData.poolTag, spawnPos, Quaternion.Euler(0, 180, 0));
    
        if (spawnedObj != null)
        {
            if (spawnedObj.TryGetComponent(out EnemyController enemyController))
            {
                enemyController.Initialize(enemyData, targetProvider);
            }
        }
    }
    
    private Vector3 GetSpawnPosition(SpawnSettings spawnSettings)
    {
        float randomX = Random.Range(
            -spawnSettings.spawnXRange,
            spawnSettings.spawnXRange);
        
        Vector3 spawnPos = new Vector3(randomX, 
            spawnSettings.spawnY, 
            transform.position.z);

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
