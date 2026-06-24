using System;
using System.Collections.Generic;

[Serializable]
public class ActiveEntitySaveData
{
    public string poolTag;
    public float posX;
    public float posY;
    public float posZ;
    public int currentHealth;
}

[Serializable]
public class GameplaySaveData
{
    public string saveSlotID;
    public string saveDate;
    
    public int currentLevelIndex;
    public float levelTimer;
    public float survivalTime;
    public int enemiesKilledScore;
    public int bonusScore;
    
    public int playerHealth;
    public float playerPosX;
    public string currentAbilityTag;

    public List<ActiveEntitySaveData> activeEnemies = new List<ActiveEntitySaveData>();
}