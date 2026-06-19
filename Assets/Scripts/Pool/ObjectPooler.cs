using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform); 
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            prefabDictionary.Add(pool.tag, pool.prefab);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }
        
        if (poolDictionary[tag].Count == 0)
        {
            GameObject obj = Instantiate(prefabDictionary[tag]);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            poolDictionary[tag].Enqueue(obj);
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
 
        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        if (!poolDictionary.ContainsKey(tag))
        {
            return;
        }
        poolDictionary[tag].Enqueue(obj);
    }
    
    public GameObject SpawnFromPoolWithPrefabRotation(string tag, Vector3 position)
    {
        if (!prefabDictionary.ContainsKey(tag))
        {
            return null;
        }

        Quaternion prefabRotation = prefabDictionary[tag].transform.rotation;
    
        return SpawnFromPool(tag, position, prefabRotation);
    }
    
    public GameObject SpawnWithDynamicPrefab(string tag, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            poolDictionary.Add(tag, new Queue<GameObject>());
            if (!prefabDictionary.ContainsKey(tag))
            {
                prefabDictionary.Add(tag, prefab);
            }
        }
    
        if (poolDictionary[tag].Count == 0)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            poolDictionary[tag].Enqueue(obj);
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }
}