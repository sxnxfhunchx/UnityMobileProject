using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public string projectilePoolTag; 
    public Transform firePoint;
    public float fireRate = 0.2f; 

    private float nextFireTime;
    private bool isShooting = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0) 
        {
            isShooting = true;
        }
        else if (context.canceled) 
        {
            isShooting = false;
        }
    }

    void Update()
    {
        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (!string.IsNullOrEmpty(projectilePoolTag) && firePoint != null && ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.SpawnFromPoolWithPrefabRotation(projectilePoolTag, firePoint.position);
        }
    }
}