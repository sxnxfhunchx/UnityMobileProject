using System;
using UnityEngine;

public class KeyboardInput : MonoBehaviour, IPlayerInput
{
    public event Action<Vector3> OnMoveInput;
    public event Action<bool> OnShootInput;
    public event Action OnDashInput;
    public event Action OnAbilityInput;

    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode shootKey = KeyCode.Space;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    private bool wasShooting;

    private void Update()
    {
        Vector3 movement = Vector2.zero;

        if (Input.GetKey(leftKey))
            movement += Vector3.left;

        if (Input.GetKey(rightKey))
            movement += Vector3.right;

        OnMoveInput?.Invoke(movement);

        bool isShooting = Input.GetKey(shootKey);

        if (isShooting != wasShooting)
        {
            OnShootInput?.Invoke(isShooting);
            wasShooting = isShooting;
        }

        if (Input.GetKeyDown(dashKey))
        {
            OnDashInput?.Invoke();
        }
    }
}
