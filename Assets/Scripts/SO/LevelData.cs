using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelSettings", menuName = "ScriptableObjects/LevelSettings")]
public class LevelSettings : ScriptableObject
{
    public int levelNumber;
    public float levelDuration = 20f;
    public float enemySpeedMultiplier = 1.0f;
    
    public SpawnSettings spawnSettings;
    
}