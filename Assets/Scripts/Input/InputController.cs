using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, IPlayerInput
{
    public event Action<Vector3> OnMoveInput;
    public event Action<bool> OnShootInput;
    public event Action OnDashInput;

    private InputSystem_Actions inputActions;
    
    public void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += Move;
        inputActions.Player.Attack.performed += Shoot;
        inputActions.Player.Attack.canceled += StopShooting;
        inputActions.Player.Dash.performed += Dash;
        inputActions.Enable();
    }

    public void Move(InputAction.CallbackContext context)
    {
        OnMoveInput?.Invoke(context.ReadValue<Vector2>());
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        OnShootInput?.Invoke(true);
    }
    
    public void StopShooting(InputAction.CallbackContext context)
    {
        OnShootInput?.Invoke(false);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        OnDashInput?.Invoke();
    }
}
