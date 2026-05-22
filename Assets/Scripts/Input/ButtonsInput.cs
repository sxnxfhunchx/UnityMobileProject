using System;
using UnityEngine;

public class ButtonsInput : MonoBehaviour, IPlayerInput
{
    public event Action<Vector3> OnMoveInput;
    public event Action<bool> OnShootInput;
    public event Action OnDashInput;

    public void MoveLeft()
    {
        OnMoveInput?.Invoke(Vector3.left);
    }

    public void MoveRight()
    {
        OnMoveInput?.Invoke(Vector3.right);
    }

    public void StopMovement()
    {
        OnMoveInput?.Invoke(Vector3.zero);
    }

    public void Shoot()
    {
        OnShootInput?.Invoke(true);
    }
    
    public void StopShooting()
    {
        OnShootInput?.Invoke(false);
    }

    public void Dash()
    {
        OnDashInput?.Invoke();
    }
}
