using System;
using SO;
using SO.PowerUps;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnSettings
{
    [Header("Spawn Area")]
    public float spawnXRange = 5f;
    public float spawnY = 0.05f;
    public float bonusSpawnY = 1f;
    
    [Header("Enemies")]
    public EnemySpawnEntry[] enemies;
    public float spawnInterval;

    [Header("Bosses")]
    public EnemyData[] bosses;
    public int bossesCount;

    [Header("Bonuses")]
    public BonusData[] bonuses;
    public float bonusSpawnInterval;
    [Range(0f, 1f)]
    public float bonusSpawnChance;
    
    [Header("PowerUps")]
    public PowerUpData[] powerUps;
    public float powerUpSpawnInterval;
    [Range(0f, 1f)]
    public float powerUpSpawnChance;
    
    public PowerUpData GetRandomPowerUp()
    { 
        return GetRandomData(powerUps) as PowerUpData;
    }
    
    public BonusData GetRandomBonus()
    { 
        return GetRandomData(bonuses) as BonusData;
    }
    
    public EnemyData GetRandomEnemy()
    {
        if (enemies == null || enemies.Length == 0)
            return null;

        float totalWeight = 0f;

        foreach (EnemySpawnEntry entry in enemies)
        {
            if (entry?.enemyData != null)
                totalWeight += entry.spawnWeight;
        }

        float roll = Random.Range(0f, totalWeight);

        foreach (EnemySpawnEntry entry in enemies)
        {
            if (entry?.enemyData == null)
                continue;

            roll -= entry.spawnWeight;

            if (roll <= 0f)
                return entry.enemyData;
        }

        return null;
    }
    
    public EnemyData GetRandomBoss()
    { 
        return GetRandomData(bosses) as EnemyData;
    }
    
    private ScriptableObject GetRandomData(ScriptableObject[] list)
    {
        if (list == null || list.Length == 0)
            return null;

        int randomIndex = Random.Range(0, list.Length);
        return list[randomIndex];
    }
}
