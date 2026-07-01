using System.Collections;
using Interfaces;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyData data; 
    [SerializeField] private SoundData soundData;
    [SerializeField] private string deathVfxPoolTag = "DeathVFX";
    [SerializeField] float destroyOnZ;
    
    private int currentHealth;
    private Renderer[] renderers;
    private Color[] originalColors;
    
    private ILevelProvider levelProvider;
    private ITargetProvider targetProvider;
    
    private Vector3 moveDirection = Vector3.back;
    private bool hasLockedTarget;
    private bool isDead = false;
    
    private Coroutine hitFlashCoroutine;
    
    public EnemyData GetEnemyData() => data;
    public int GetCurrentHealth() => currentHealth;

    public void Initialize(EnemyData newData, ITargetProvider targetProvider)
    {
        isDead = false;
        data = newData;
        this.targetProvider = targetProvider;

        if (data == null)
            return;

        float healthMultiplier = levelProvider != null
            ? levelProvider.CurrentLevelSettings.enemyHealthMultiplier
            : 1f;

        currentHealth = Mathf.RoundToInt(data.health * healthMultiplier);
        
        hasLockedTarget = false;
        moveDirection = Vector3.back;
        
        ApplyEnemySettings();
    }

    void Awake()
    {
        levelProvider = FindFirstObjectByType<LevelManager>();
        
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                originalColors[i] = renderers[i].material.color;
        }
    }
    
    private void Update()
    {
        if (data == null)
            return;
    
        UpdateBerserkDirection();
        
        float speedMultiplier = 1f;

        if (levelProvider != null)
        {
            speedMultiplier = levelProvider.CurrentLevelSettings.enemySpeedMultiplier;
        }

        float difficultyMultiplier = GameManager.Instance.GetDifficultyEnemySpeedMultiplier();

        float finalSpeed = data.speed * speedMultiplier * difficultyMultiplier;

        if (hasLockedTarget)
        {
            float zDistanceToTarget = transform.position.z - targetProvider.Target.position.z;

            if (zDistanceToTarget <= 0f)
            {
                moveDirection = Vector3.back;
                hasLockedTarget = false;
            }
        }
        
        transform.Translate(moveDirection * (finalSpeed * Time.deltaTime), Space.World);

        if (transform.position.z < destroyOnZ)
        {
            ReturnToPool();
        }
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        PlayHitSound();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartHitFlash();
    }
    
    private void StartHitFlash()
    {
        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlash());
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.Instance != null && data != null)
        {
            GameManager.Instance.AddEnemyKillScore(data.scoreValue); 
        }
    
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.TrackProgress(QuestType.KillEnemies, 1);
        }
    
        SpawnDeathVFX(); 
        ReturnToPool(); 
    }

    private void SpawnDeathVFX()
    {
        if (ObjectPooler.Instance == null)
            return;
        
        if (string.IsNullOrEmpty(deathVfxPoolTag))
            return;
        
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        ObjectPooler.Instance.SpawnFromPool(deathVfxPoolTag, 
            spawnPos, Quaternion.identity);
    }

    private void PlayHitSound()
    {
        if (SoundManager.Instance == null || soundData == null)
            return;
        
        AudioClip clip = soundData.GetRandomSound();
        
        if (clip == null)
            return;
        
        SoundManager.Instance.PlaySound(clip, transform.position);
    }

    private void ReturnToPool()
    {
        if (ObjectPooler.Instance == null || data == null)
            return;
        
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
            hitFlashCoroutine = null;
        }

        RestoreOriginalColors();

        string tagToReturn = data.poolTag;

        data = null;

        ObjectPooler.Instance.ReturnToPool(tagToReturn, gameObject);
    }

    private IEnumerator HitFlash()
    {
        foreach (var r in renderers)
        {
            if (r != null) r.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        ApplyDataColors();
    }
    
    private void RestoreOriginalColors()
    {
        if (renderers != null && originalColors != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null) renderers[i].material.color = originalColors[i];
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isDead || data == null) return;
        
        if (!other.TryGetComponent(out PlayerHealth player))
            return;

        var abilityController = player.GetComponentInChildren<Ability.PlayerAbilityController>();
        if (abilityController != null && abilityController.TryGetCurrentAbility<Ability.ShieldAbility>(out var shield) && shield.IsActive)
        {
            if (shield.TryBlockDamage())
            {
                Die(); 
                return;
            }
        }

        float damageMultiplier = levelProvider != null ? 
            levelProvider.CurrentLevelSettings.enemyDamageMultiplier : 
            1f;
        
        float difficultyMultiplier = GameManager.Instance.GetDifficultyEnemyDamageMultiplier();
        
        int finalDamage = Mathf.RoundToInt(data.damage * damageMultiplier * difficultyMultiplier);

        player.TakeDamage(finalDamage);

        ObjectPooler.Instance.ReturnToPool(data.poolTag, gameObject);
    }
    
    private void UpdateBerserkDirection()
    {
        if (data == null)
            return;

        if (!data.isBerserkMode)
            return;

        if (hasLockedTarget)
            return;

        if (targetProvider?.Target == null)
            return;
        
        float zDistanceToTarget = transform.position.z - targetProvider.Target.position.z;

        if (zDistanceToTarget > data.berserkDetectionDistance)
            return;
        
        if (zDistanceToTarget < 0.5f)
            return;

        Vector3 direction = targetProvider.Target.position - transform.position;

        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        moveDirection = direction.normalized;
        hasLockedTarget = true;
    }
    
    public void SetSavedHealth(int savedHealth)
    {
        currentHealth = savedHealth;
    }

    public void ApplyEnemySettings()
    {
        if (data == null) 
            return;

        ApplyDataColors();
    }
    
    private void ApplyDataColors()
    {
        RestoreOriginalColors();

        if (data != null && data.isBerserkMode && renderers != null)
        {
            foreach (var r in renderers)
            {
                if (r != null)
                    r.material.color = data.berserkColorTint;
            }
            
        }
    }

}
