using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovementNew : MonoBehaviour
{
    [Header("Movement Settings")]
    public float sensitivity = 10f; 
    public float minX = -5f;
    public float maxX = 5f;

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

        newX = Mathf.Clamp(newX, minX, maxX);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}