using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private MonoBehaviour inputSource;
    
    [Header("Movement Settings")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 2f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;
    
    private float speed = 5f;
    private Vector3 moveInput;
    private float lastInputX;
    private IPlayerInput playerInput;
    
    private bool canDash = true;
    private bool isDashing;
    private Coroutine dashCoroutine;
    
    public event Action<bool> OnDashAvailabilityChanged;

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
        if (input.x != 0)
            lastInputX = input.x;
    }

    private void Update()
    {
        if (isDashing)
            return;
        
        if (moveInput == Vector3.zero)
            return;
        
        float x = transform.position.x + moveInput.x * speed * Time.deltaTime;
        x = Mathf.Clamp(x, minX, maxX);
        
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void Dash()
    {
        if (!canDash || isDashing)
            return;

        //if (moveInput == Vector3.zero)
            //return;

        dashCoroutine = StartCoroutine(DashCoroutine(lastInputX));
    }
    
    private IEnumerator DashCoroutine(float directionX)
    {
        SetCanDash(false);
        isDashing = true;

        float startX = transform.position.x;
        float targetX = startX + directionX * dashDistance;
        targetX = Mathf.Clamp(targetX, minX, maxX);

        float timer = 0f;

        while (timer < dashDuration)
        {
            timer += Time.deltaTime;

            float t = timer / dashDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float x = Mathf.Lerp(startX, targetX, t);

            transform.position = new Vector3(
                x,
                transform.position.y,
                transform.position.z
            );

            yield return null;
        }

        transform.position = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        SetCanDash(true);
    }
    
    private void OnValidate()
    {
        if (inputSource != null && inputSource is not IPlayerInput)
        {
            Debug.LogWarning("Input source must implement IPlayerInput");
            inputSource = null;
        }
    }
    
    private void SetCanDash(bool value)
    {
        if (canDash == value)
            return;

        canDash = value;
        OnDashAvailabilityChanged?.Invoke(canDash);
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}