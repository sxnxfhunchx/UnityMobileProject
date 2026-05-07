using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelSettings", menuName = "ScriptableObjects/LevelSettings")]
public class LevelSettings : ScriptableObject
{
    public int levelNumber;
    public float spawnInterval = 1.5f;
    public float levelDuration = 20f;
    public float enemySpeedMultiplier = 1.0f;
}