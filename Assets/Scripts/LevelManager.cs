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
        Vector3 spawnPos = new Vector3(randomX, -0.09f, transform.position.z);

        GameObject newEnemy = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], spawnPos, Quaternion.identity);
    
       
        newEnemy.transform.rotation = Quaternion.Euler(0, 180, 0); 
    }
}