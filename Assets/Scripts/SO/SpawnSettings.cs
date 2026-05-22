using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnSettings
{
        [Header("Enemies")]
        public EnemyData[] enemies;
        public float enemySpawnInterval;

        [Header("Bosses")]
        public EnemyData[] bosses;
        public int bossesCount;

        [Header("Bonuses")]
        public BonusData[] bonuses;
        public float bonusSpawnInterval;
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
            if (list.Length == 0) 
                return null;

            int randomIndex = Random.Range(0, list.Length);
            return list[randomIndex];
        }
}
