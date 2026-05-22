using System;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusSpawner : MonoBehaviour
{
    [SerializeField] private MonoBehaviour levelProvider;

    private float bonusTimer;
    private ILevelProvider LevelProvider;
    
    private void Awake()
    {
        LevelProvider = levelProvider as ILevelProvider;

        if (LevelProvider == null)
            Debug.LogError("Level Spawn Provider must implement ILevelProvider");
    }
    
    public void SpawnBonus(BonusData bonusData)
    {
        if (ObjectPooler.Instance == null)
            return;
        
        float randomX = Random.Range(-5f, 5f);
        Vector3 spawnPos = new Vector3(randomX, 1, transform.position.z);
        
        ObjectPooler.Instance.SpawnFromPool(bonusData.bonusName, spawnPos, Quaternion.Euler(0, 180, 0));
    }

    private void Update()
    {
        bonusTimer += Time.deltaTime;

        SpawnSettings spawnSettings = LevelProvider.CurrentLevelSettings.spawnSettings;
        
        if (bonusTimer >= spawnSettings.bonusSpawnInterval)
        {
            bonusTimer = 0f;

            if (Random.value <= spawnSettings.bonusSpawnChance)
            {
                SpawnBonus(spawnSettings.GetRandomBonus());
            }
        }
    }
}
