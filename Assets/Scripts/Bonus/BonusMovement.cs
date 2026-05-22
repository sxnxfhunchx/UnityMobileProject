using UnityEngine;

public class BonusMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float destroyZ = -10f;

    private void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;

        if (transform.position.z <= destroyZ)
        {
            gameObject.SetActive(false);
        }
    }
}
