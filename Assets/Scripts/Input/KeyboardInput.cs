using System;
using UnityEngine;

public class KeyboardInput : MonoBehaviour, IPlayerInput
{
    public event Action<Vector3> OnMoveInput;
    public event Action<bool> OnShootInput;

    private bool wasShooting;
    
    private void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
            movement += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            movement += Vector3.right;

        Debug.Log(Input.GetKey(KeyCode.A));
        
        OnMoveInput?.Invoke(movement);

        bool isShooting = Input.GetKey(KeyCode.Space);

        if (isShooting != wasShooting)
        {
            OnShootInput?.Invoke(isShooting);
            wasShooting = isShooting;
        }
    }
}
