using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] MonoBehaviour inputSource;
    
    [Header("Movement Settings")]
    [SerializeField] float speed;
    [SerializeField]  float minX = -5f;
    [SerializeField]  float maxX = 5f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 2f;
    [SerializeField] private float dashCooldown = 1f;
    
    private Vector2 moveInput;
    private IPlayerInput playerInput;
    private bool canDash = true;

    private void Awake()
    {
        playerInput = inputSource as IPlayerInput;
        
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput is not set");
            return;
        }

        playerInput.OnMoveInput += SetMoveInput;
        playerInput.OnDashInput += Dash;
    }

    private void OnDestroy()
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
        if (moveInput != Vector2.zero)
        {
            float x = transform.position.x + moveInput.x * speed * Time.deltaTime;
            x = Mathf.Clamp(x, minX, maxX);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }

    private void Dash()
    {
        if (!canDash && moveInput == Vector2.zero)
            return;

        StartCoroutine(DashCoroutine());
    }
    
    private IEnumerator DashCoroutine()
    {
        canDash = false;

        Debug.Log("Dashing");
        
        float x = transform.position.x + moveInput.normalized.x * dashDistance;
        x = Mathf.Clamp(x, minX, maxX);
        
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

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