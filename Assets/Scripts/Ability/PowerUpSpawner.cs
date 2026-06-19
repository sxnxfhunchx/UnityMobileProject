using Interfaces;
using SO.PowerUps;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private MonoBehaviour levelProviderSource;
    [SerializeField] private string powerUpPoolTag = "PowerUp";

    private ILevelProvider levelProvider;
    private float powerUpTimer;

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

        HandlePowerUpSpawning(spawnSettings);
    }

    private void HandlePowerUpSpawning(SpawnSettings spawnSettings)
    {
        powerUpTimer += Time.deltaTime;

        if (powerUpTimer < spawnSettings.powerUpSpawnInterval)
            return;

        powerUpTimer = 0f;

        float difficultyMultiplier = GameManager.Instance.GetDifficultyPowerUpMultiplier();
        
        if (Random.value > spawnSettings.powerUpSpawnChance * difficultyMultiplier)
            return;

        PowerUpData powerUpData = spawnSettings.GetRandomPowerUp();

        if (powerUpData == null)
            return;

        SpawnPowerUp(spawnSettings, powerUpData);
    }

    private void SpawnPowerUp(SpawnSettings spawnSettings, PowerUpData powerUpData)
    {
        if (powerUpData == null || powerUpData.Prefab == null)
        {
            return;
        }

        string uniqueTag = string.IsNullOrEmpty(powerUpData.poolTag) ? powerUpData.name : powerUpData.poolTag;

        Quaternion spawnRotation = powerUpData.Prefab.transform.rotation; 

        GameObject obj = ObjectPooler.Instance.SpawnWithDynamicPrefab(
            uniqueTag, 
            powerUpData.Prefab, 
            GetSpawnPosition(spawnSettings), 
            spawnRotation
        );
    
        if (obj == null)
            return;
    
        if (obj.TryGetComponent(out PowerUpPickup pickup))
            pickup.Initialize(powerUpData);
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
