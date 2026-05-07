using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovementNew : MonoBehaviour
{
    [Header("Movement Settings")]
    public float sensitivity = 0.5f;
    public Vector2 minBounds = new Vector2(-5, 0.5f);
    public Vector2 maxBounds = new Vector2(5, 8);

    private Vector2 moveInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (moveInput != Vector2.zero)
        {
            MovePlayer(moveInput);
        }
    }

    private void MovePlayer(Vector2 delta)
    {
        float newX = transform.position.x + delta.x * sensitivity * Time.deltaTime;
        float newY = transform.position.y + delta.y * sensitivity * Time.deltaTime;

        newX = Mathf.Clamp(newX, minBounds.x, maxBounds.x);
        newY = Mathf.Clamp(newY, minBounds.y, maxBounds.y);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}