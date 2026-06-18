using System;

namespace SO
{
    [Serializable]
    public class EnemySpawnEntry
    {
        public EnemyData enemyData;
        public float spawnWeight = 1f;
    }
}