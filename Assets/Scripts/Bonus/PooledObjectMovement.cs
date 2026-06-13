using UnityEngine;

public class PooledObjectMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float destroyZ = -10f;
    [SerializeField] private string poolTag;

    private void Update()
    {
        transform.position += Vector3.back * (moveSpeed * Time.deltaTime);

        if (transform.position.z <= destroyZ)
        {
            ReturnToPool();
        }
    }
    
    private void ReturnToPool()
    {
        if (ObjectPooler.Instance == null || poolTag == null)
            return;

        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}
