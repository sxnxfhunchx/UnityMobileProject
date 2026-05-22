using Interfaces;
using UnityEngine;

public class TestSpawn : MonoBehaviour, ILevelProvider
{

    [SerializeField] private LevelSettings testSettings;

    public LevelSettings CurrentLevelSettings => testSettings;
    public bool IsRegularEnemyPhaseActive => true;
    public bool IsBossPhaseActive => Time.time % 20f >= 15f && Time.time % 20f < 17f;
    
}
