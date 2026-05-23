using System;
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
    public EnemyData[] enemies;
    public float spawnInterval;

    [Header("Bosses")]
    public EnemyData[] bosses;
    public int bossesCount;

    [Header("Bonuses")]
    public BonusData[] bonuses;
    public float bonusSpawnInterval;
    [Range(0f, 1f)]
    public float bonusSpawnChance;
    
    public BonusData GetRandomBonus()
    { 
        return GetRandomData(bonuses) as BonusData;
    }
    
    public EnemyData GetRandomEnemy()
    { 
        return GetRandomData(enemies) as EnemyData;
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
