using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; 
    public GameObject bossPrefab; 
    
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
        
        GameObject prefabToSpawn;

        if (isBoss)
        {
            prefabToSpawn = bossPrefab;
        }
        else
        {
            if (obstaclePrefabs.Length == 0) return;
            prefabToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPos, Quaternion.Euler(0, 180, 0));
        }
    }
}