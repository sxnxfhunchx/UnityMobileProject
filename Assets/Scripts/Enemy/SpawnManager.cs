using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public string[] obstaclePoolTags; 
    public string bossPoolTag; 
    
    private float currentSpawnInterval = 1.5f;
    private float timer;
    private bool isSpawning = true;

    void Update()
    {
        if (!isSpawning) return;

        timer += Time.deltaTime;
        if (timer >= currentSpawnInterval)
        {
            SpawnObstacle(false); 
            timer = 0;
        }
    }

    public void UpdateSettings(float interval)
    {
        currentSpawnInterval = interval;
    }

    public void SetSpawning(bool state)
    {
        isSpawning = state;
    }

    public void SpawnObstacle(bool isBoss)
    {
        float randomX = Random.Range(-5f, 5f);
        Vector3 spawnPos = new Vector3(randomX, -0.09f, transform.position.z);
        
        string poolTagToSpawn;

        if (isBoss)
        {
            poolTagToSpawn = bossPoolTag;
        }
        else
        {
            if (obstaclePoolTags.Length == 0) return;
            poolTagToSpawn = obstaclePoolTags[Random.Range(0, obstaclePoolTags.Length)];
        }

        if (!string.IsNullOrEmpty(poolTagToSpawn) && ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.SpawnFromPool(poolTagToSpawn, spawnPos, Quaternion.Euler(0, 180, 0));
        }
    }
}