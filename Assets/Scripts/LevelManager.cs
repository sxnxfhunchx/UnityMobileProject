using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; 
    public float spawnInterval = 1.5f;

    void Start()
    {
        InvokeRepeating("SpawnObstacle", 0f, spawnInterval);
    }

    void SpawnObstacle()
    {
        float randomX = Random.Range(-5f, 5f);
        float randomY = Random.Range(0f, 3f);
        Vector3 spawnPos = new Vector3(randomX, randomY, transform.position.z);

        Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], spawnPos, Quaternion.identity);
    }
}