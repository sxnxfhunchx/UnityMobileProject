using System.Collections;
using Interfaces;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private SoundData soundData;
    [SerializeField] private string deathVfxPoolTag = "DeathVFX";
    [SerializeField] float destroyOnZ;
    
    private int currentHealth;
    private Renderer[] renderers;
    private Color[] originalColors;
    
    private ILevelProvider levelProvider;

    void Awake()
    {
        levelProvider = FindFirstObjectByType<LevelManager>();
        
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    void OnEnable()
    {
        if (data != null) 
            currentHealth = data.health;

        RestoreOriginalColors();
    }
    
    private void Update()
    {
        if (data == null)
            return;
    
        float speedMultiplier = 1f;

        if (levelProvider != null)
        {
            speedMultiplier = levelProvider.CurrentLevelSettings.enemySpeedMultiplier;
        }

        int difficulty = PlayerPrefs.GetInt("GameDifficulty", 2);
        float difficultyMultiplier = 1f;

        if (difficulty == 1) difficultyMultiplier = 0.75f; 
        if (difficulty == 3) difficultyMultiplier = 1.35f; 

        float finalSpeed = data.speed * speedMultiplier * difficultyMultiplier;

        transform.Translate(Vector3.back * (finalSpeed * Time.deltaTime), Space.World);

        if (transform.position.z < destroyOnZ)
        {
            ReturnToPool();
        }
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        PlayHitSound();

        StartCoroutine(HitFlash());

        if (currentHealth <= 0) 
            Die();
    }

    private void Die()
    {
        if (GameManager.Instance != null && data != null)
        {
            GameManager.Instance.AddEnemyKillScore(data.scoreValue);
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
        
        ObjectPooler.Instance.ReturnToPool(data.poolTag, gameObject);
    }

    private IEnumerator HitFlash()
    {
        foreach (var r in renderers)
        {
            if (r != null) r.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        RestoreOriginalColors();
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
        if (data == null)
            return;
        
        if (!other.TryGetComponent(out PlayerHealth player))
            return;

        player.TakeDamage(data.damage);

        ObjectPooler.Instance.ReturnToPool(data.poolTag, gameObject);
    }
    
}
