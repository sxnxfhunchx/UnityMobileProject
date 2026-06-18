using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusSpawner : MonoBehaviour
{
    [SerializeField] private MonoBehaviour levelProviderSource;

    private ILevelProvider levelProvider;
    private float bonusTimer;

    private void Awake()
    {
        levelProvider = levelProviderSource as ILevelProvider;

        if (levelProvider == null)
            Debug.LogError("Level Spawn Provider must implement ILevelProvider");
    }
    
    private void Update()
    {
        if (levelProvider == null)
            return;

        SpawnSettings spawnSettings = levelProvider.CurrentLevelSettings.spawnSettings;
        
        if (spawnSettings == null)
            return;
        
        HandleBonusSpawning(spawnSettings);
    }

    private void HandleBonusSpawning(SpawnSettings spawnSettings)
    {
        bonusTimer += Time.deltaTime;
        
        if (bonusTimer < spawnSettings.bonusSpawnInterval)
            return;
        
        bonusTimer = 0f;

        float difficultyMultiplier = GameManager.Instance.GetDifficultyPowerUpMultiplier();
        if (Random.value > spawnSettings.bonusSpawnChance * difficultyMultiplier)
            return;

        BonusData bonusData = spawnSettings.GetRandomBonus();
        
        if (bonusData == null)
            return;
        
        SpawnBonus(bonusData, spawnSettings);
    }

    public void SpawnBonus(BonusData bonusData, SpawnSettings spawnSettings)
    {
        if (ObjectPooler.Instance == null)
            return;
        
        if (string.IsNullOrEmpty(bonusData.poolTag))
            return;

        Vector3 spawnPosition = GetSpawnPosition(spawnSettings);

        ObjectPooler.Instance.SpawnFromPool(bonusData.bonusName,
            spawnPosition, Quaternion.Euler(0, 180, 0));
    }

    private Vector3 GetSpawnPosition(SpawnSettings spawnSettings)
    {
        float randomX = Random.Range(
            -spawnSettings.spawnXRange,
            spawnSettings.spawnXRange
        );

        return new Vector3(
            randomX,
            spawnSettings.bonusSpawnY,
            transform.position.z
        );
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
