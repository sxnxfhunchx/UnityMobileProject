using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] MonoBehaviour inputSource;
    
    [Header("Shoot Settings")]
    public string projectilePoolTag; 
    public Transform firePoint;
    public float fireRate = 0.2f; 

    private float nextFireTime;
    private bool isShooting = false;
    private IPlayerInput playerInput;
    
    
    private void Awake()
    {
        playerInput = inputSource as IPlayerInput;
        
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput is not set");
            return;
        }

        playerInput.OnShootInput += ToggleShooting;
    }

    private void Update()
    {
        if (isShooting && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ToggleShooting(bool state)
    {
        isShooting = state;
    }

    private void Shoot()
    {
        if (string.IsNullOrEmpty(projectilePoolTag))
            return;
        
        if (!firePoint)
            return;
        
        if (!ObjectPooler.Instance)
            return;
        
        ObjectPooler.Instance.SpawnFromPoolWithPrefabRotation(projectilePoolTag, 
                                                            firePoint.position);
    }
    
    private void OnDestroy()
    {
        if (playerInput == null)
            return;
        
        playerInput.OnShootInput -= ToggleShooting;
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