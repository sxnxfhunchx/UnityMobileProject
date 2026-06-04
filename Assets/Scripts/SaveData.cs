using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySaveData
{
    public string poolTag;
    public float posX;
    public float posY;
    public float posZ;
    public int health; 
}

[System.Serializable]
public class BonusSaveData
{
    public string poolTag;
    public float posX;
    public float posY;
    public float posZ;
}

[System.Serializable]
public class GameSaveRoot
{
    public int bonusScore;
    public int enemiesKilledScore;
    public float survivalTime;

    public int playerCurrentHealth;
    public float playerPosX;

    public int currentLevelIndex;
    public float levelTimer;
    public bool isBossPhaseActive;

    public List<EnemySaveData> activeEnemies = new List<EnemySaveData>();
    public List<BonusSaveData> activeBonuses = new List<BonusSaveData>();
}