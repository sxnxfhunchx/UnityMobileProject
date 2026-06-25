using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Pool Tags")]
    [SerializeField] private string[] treeTags = { "Tree01", "Tree02", "Tree03", "Tree03", "Tree04", "Tree05", "Tree06", "Tree07", "Tree08", "Tree09", "Tree10" };

    [Header("Spawn Settings")]
    [SerializeField] private float spawnIntervalZ = 5f; 
    [SerializeField] private float spawnZ = 60f;        
    [SerializeField] private float destroyZ = -10f;     

    [Header("Side Offsets (X axis)")]
    [SerializeField] private float minLeftX = -15f;     
    [SerializeField] private float maxLeftX = -8f;
    [SerializeField] private float minRightX = 8f;      
    [SerializeField] private float maxRightX = 15f;

    private float nextSpawnZ;
    private List<GameObject> activeObjects = new List<GameObject>();

    void Start()
    {
        nextSpawnZ = spawnZ;
        
        for (float z = destroyZ + 5f; z < spawnZ; z += spawnIntervalZ)
        {
            SpawnTreePair(z);
        }
    }

    void Update()
    {
        
        MoveAndCheckActiveObjects();
    }

    public void SpawnTreePair(float zPosition)
    {
        if (ObjectPooler.Instance == null || treeTags.Length == 0) return;

        string leftTreeTag = treeTags[Random.Range(0, treeTags.Length)];
        string rightTreeTag = treeTags[Random.Range(0, treeTags.Length)];

        float leftX = Random.Range(minLeftX, maxLeftX);
        float rightX = Random.Range(minRightX, maxRightX);

        Vector3 leftPos = new Vector3(leftX, 0f, zPosition);
        Vector3 rightPos = new Vector3(rightX, 0f, zPosition);

        Quaternion randomRotationLeft = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        Quaternion randomRotationRight = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        GameObject leftTree = ObjectPooler.Instance.SpawnFromPool(leftTreeTag, leftPos, randomRotationLeft);
        GameObject rightTree = ObjectPooler.Instance.SpawnFromPool(rightTreeTag, rightPos, randomRotationRight);

        if (leftTree != null) activeObjects.Add(leftTree);
        if (rightTree != null) activeObjects.Add(rightTree);
    }

    private void MoveAndCheckActiveObjects()
    {
        
        float currentSpeed = GameManager.Instance != null ? GameManager.Instance.GetDifficultyEnemySpeedMultiplier() * 5f : 5f; 

        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = activeObjects[i];
            
            if (obj == null || !obj.activeSelf)
            {
                activeObjects.RemoveAt(i);
                continue;
            }

            obj.transform.Translate(Vector3.back * (currentSpeed * Time.deltaTime), Space.World);

            if (obj.transform.position.z < destroyZ)
            {
               
                EnvironmentObject envScript = obj.GetComponent<EnvironmentObject>();
                if (envScript != null)
                {
                    ObjectPooler.Instance.ReturnToPool(envScript.poolTag, obj);
                }
                else
                {
                    obj.SetActive(false); 
                }

                activeObjects.RemoveAt(i);
            }
        }

        
    }
}