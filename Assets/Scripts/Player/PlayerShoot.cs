using UnityEngine;


public class PlayerShoot : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private MonoBehaviour inputSource;
    
    [Header("Shoot Settings")]
    [SerializeField] private string projectilePoolTag; 
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f; 

    private float nextFireTime;
    private bool isShooting = false;
    private IPlayerInput playerInput;
    
    private void OnEnable()
    {
        playerInput = inputSource as IPlayerInput;
        
        if (playerInput == null)
        {
            Debug.LogWarning("Input source must implement IPlayerInput");
            return;
        }

        playerInput.OnShootInput += SetShooting;
    }
    
    private void OnDisable()
    {
        if (playerInput == null)
            return;
        
        playerInput.OnShootInput -= SetShooting;
    }

    private void Update()
    {
        if (isShooting && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void SetShooting(bool state)
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
        
        ObjectPooler.Instance.SpawnFromPoolWithPrefabRotation(projectilePoolTag, firePoint.position);
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