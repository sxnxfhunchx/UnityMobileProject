using UnityEngine;

public class PooledObjectMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float destroyZ = -10f;
    private string currentPoolTag;

    public void SetPoolTag(string tag)
    {
        currentPoolTag = tag;
    }

    private void Update()
    {
        transform.position += Vector3.back * (moveSpeed * Time.deltaTime);

        if (transform.position.z <= destroyZ)
        {
            ReturnToPool();
        }
    }
    
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(currentPoolTag))
        {
            currentPoolTag = gameObject.name.Replace("(Clone)", "").Trim();
        }
    }
    
    private void ReturnToPool()
    {
        if (ObjectPooler.Instance == null || string.IsNullOrEmpty(currentPoolTag))
            return;

        ObjectPooler.Instance.ReturnToPool(currentPoolTag, gameObject);
    }
}