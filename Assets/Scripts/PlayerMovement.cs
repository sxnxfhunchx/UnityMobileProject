using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float sensitivity = 0.05f; 
    public Vector2 minBounds = new Vector2(-5, 0);
    public Vector2 maxBounds = new Vector2(5, 8);  

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float newX = transform.position.x + touch.deltaPosition.x * sensitivity;
                float newY = transform.position.y + touch.deltaPosition.y * sensitivity;

                newX = Mathf.Clamp(newX, minBounds.x, maxBounds.x);
                newY = Mathf.Clamp(newY, minBounds.y, maxBounds.y);

                transform.position = new Vector3(newX, newY, transform.position.z);
            }
        }
    }
}