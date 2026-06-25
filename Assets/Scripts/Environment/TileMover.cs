using System.Collections.Generic;
using UnityEngine;

public class TileMover : MonoBehaviour
{
    [SerializeField] public float speed = 20f;
    [SerializeField] public float tileLength = 30f; 
    [SerializeField] public int totalTiles = 4;

    [Header("Environment Settings")]
    [SerializeField] private Transform[] treeSpawnPoints; 
    [SerializeField] private string[] treeTags = { "Tree01", "Tree02", "Tree03", "Tree04", "Tree05", "Tree06", "Tree07", "Tree08", "Tree09", "Tree10" };

    private List<GameObject> spawnedTrees = new List<GameObject>();

    void Start()
    {
        PopulateTrees();
    
      
        foreach (GameObject tree in spawnedTrees)
        {
            if (tree != null)
            {
                tree.transform.localScale = Vector3.one;
            }
        }
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z <= -tileLength)
        {
            ClearTrees();

            Vector3 newPos = transform.position;
            newPos.z += tileLength * totalTiles;
            transform.position = newPos;

            PopulateTrees();
        }
    }

    private void PopulateTrees()
    {
        if (ObjectPooler.Instance == null || treeTags.Length == 0 || treeSpawnPoints == null) return;

        foreach (Transform point in treeSpawnPoints)
        {
            if (Random.value > 0.7f) continue;

            string randomTag = treeTags[Random.Range(0, treeTags.Length)];

            Vector3 finalPos = point.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-2f, 2f));
            
            Quaternion randomRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            GameObject tree = ObjectPooler.Instance.SpawnFromPool(randomTag, finalPos, randomRot);

            if (tree != null)
            {
                tree.transform.SetParent(this.transform);
                spawnedTrees.Add(tree);
            }
        }
    }

    private void ClearTrees()
    {
        for (int i = spawnedTrees.Count - 1; i >= 0; i--)
        {
            GameObject tree = spawnedTrees[i];
            if (tree != null)
            {
                EnvironmentObject envObj = tree.GetComponent<EnvironmentObject>();
                string tagToReturn = envObj != null ? envObj.poolTag : "Tree01"; 

                ObjectPooler.Instance.ReturnToPool(tagToReturn, tree);
            }
        }
        spawnedTrees.Clear();
    }
}