using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    public Transform target;       
    public Vector3 offset = new Vector3(0, 5, -10); 
    public float smoothness = 5f; 

    void LateUpdate()
    {
        if (target == null) return;

       
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z);
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}