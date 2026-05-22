using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private MonoBehaviour inputSource;
    
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 1f;
    
    private Vector3 moveInput;
    private IPlayerInput playerInput;
    private bool canDash = true;
    private bool isDashing = false;

    private void OnEnable()
    {
        playerInput = inputSource as IPlayerInput;

        if (playerInput == null)
        {
            Debug.LogError("Input source must implement IPlayerInput");
            return;
        }

        playerInput.OnMoveInput += SetMoveInput;
        playerInput.OnDashInput += Dash;
    }
    
    private void OnDisable()
    {
        if (playerInput == null)
            return;

        playerInput.OnMoveInput -= SetMoveInput;
        playerInput.OnDashInput -= Dash;
    }

    private void SetMoveInput(Vector3 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        if (moveInput == Vector3.zero)
            return;
        
        float currentSpeed = isDashing ? dashSpeed : speed;
        
        float x = transform.position.x + moveInput.x * currentSpeed * Time.deltaTime;
        x = Mathf.Clamp(x, minX, maxX);
        
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void Dash()
    {
        if (!canDash)
            return;

        if (moveInput == Vector3.zero)
            return;
        
        StartCoroutine(DashCoroutine());
    }
    
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
    
    private void OnValidate()
    {
        if (inputSource != null && inputSource is not IPlayerInput)
        {
            Debug.LogWarning("Input source must implement IPlayerInput");
            inputSource = null;
        }
    }
}