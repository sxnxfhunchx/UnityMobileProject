using Ability;
using UnityEngine;


public class PlayerShoot : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private MonoBehaviour inputSource;
    
    [Header("Shoot Settings")]
    [SerializeField] private string projectilePoolTag; 
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f; 
    
    [SerializeField] private AudioClip shootSound;
    
    [SerializeField] private PlayerAbilityController abilityController;
    
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
        
        if (abilityController.TryGetCurrentAbility<TripleShotAbility>(out var tripleShot)
            && tripleShot.IsActive)
        {
            ShootTriple(tripleShot.ProjectileCount, tripleShot.SpreadAngle);
            return;
        }

        ShootSingle();
    }

    private void ShootTriple(int count, float angle)
    {
        if (count <= 1)
        {
            ShootSingle();
            return;
        }

        float startAngle = -angle * (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            float currentAngle = startAngle + angle * i;
            SpawnProjectile(currentAngle);
        }
    }

    private void ShootSingle()
    {
        SpawnProjectile(0f);
    }
    
    private void SpawnProjectile(float angle)
    {
        GameObject projectile = ObjectPooler.Instance.SpawnFromPoolWithPrefabRotation(projectilePoolTag, firePoint.position);

        if (projectile == null)
            return;

        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation * Quaternion.Euler(0f, angle, 0f);

        projectile.SetActive(true);
        SoundManager.Instance?.PlaySound(shootSound, transform.position);
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