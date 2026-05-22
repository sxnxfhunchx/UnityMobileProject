using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bonusPrefab;
    [SerializeField] private float spawnChance;
    [SerializeField] private float bonusSpawnInterval;

    private float bonusTimer;
    
    public void SpawnBonus()
    {
        if (ObjectPooler.Instance == null)
            return;
        
        BonusController bonusPickup = bonusPrefab.GetComponent<BonusController>();
        
        float randomX = Random.Range(-5f, 5f);
        Vector3 spawnPos = new Vector3(randomX, 1, transform.position.z);
        
        ObjectPooler.Instance.SpawnFromPool(bonusPickup.GetTag(), spawnPos, Quaternion.Euler(0, 180, 0));
    }

    private void Update()
    {
        bonusTimer += Time.deltaTime;

        if (bonusTimer >= bonusSpawnInterval)
        {
            bonusTimer = 0f;

            if (Random.value <= spawnChance)
            {
                SpawnBonus();
            }
        }
    }
}
