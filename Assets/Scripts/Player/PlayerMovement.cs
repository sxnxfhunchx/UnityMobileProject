using System;
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
    
    private Vector2 moveInput;
    private IPlayerInput playerInput;

    private void Awake()
    {
        playerInput = inputSource as IPlayerInput;
        
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput is not set");
            return;
        }

        playerInput.OnMoveInput += SetMoveInput;
    }

    private void OnDestroy()
    {
        if (playerInput == null)
            return;
        
        playerInput.OnMoveInput -= SetMoveInput;
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
    
    private void OnValidate()
    {
        if (inputSource != null && inputSource is not IPlayerInput)
        {
            Debug.LogWarning("Input source must implement IPlayerInput");
            inputSource = null;
        }
    }
}