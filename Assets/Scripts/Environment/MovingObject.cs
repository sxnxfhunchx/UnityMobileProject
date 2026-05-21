using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 15f;
    public string poolTag;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z < -10f)
        {
            if (ObjectPooler.Instance != null && !string.IsNullOrEmpty(poolTag))
            {
                ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}