using System;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private SoundData soundData; 
    [SerializeField] string poolTag;
    [SerializeField] float destroyOnZ;
    
    private int currentHealth;
    
    private Renderer[] renderers;
    private Color[] originalColors;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    void OnEnable()
    {
        if (data != null) currentHealth = data.health;
        
        if (renderers != null && originalColors != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null) renderers[i].material.color = originalColors[i];
            }
        }
    }
    
    private void Start()
    {
        if (data != null)
            currentHealth = data.health;
    }

    private void Update()
    {
        if (data == null)
            return;
        
        transform.Translate(Vector3.back * (data.speed * Time.deltaTime), Space.World);

        if (transform.position.z < destroyOnZ)
        {
            ReturnToPool();
        }
    }
    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (SoundManager.Instance != null && soundData != null)
        {
            AudioClip clip = soundData.GetRandomHitSound();
            SoundManager.Instance.PlaySound(clip, transform.position);
        }

        StartCoroutine(HitFlash());

        if (currentHealth <= 0) Die();
    }
    
    private void Die()
    {
        if (GameManager.Instance != null)
        {
            // TODO: think about it
            GameManager.Instance.AddScore(data.scoreValue);
        }

        // TODO: find out
        if (ObjectPooler.Instance != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            ObjectPooler.Instance.SpawnFromPool("DeathVFX", spawnPos, Quaternion.identity);
        }

        ReturnToPool();
    }
    
   private void ReturnToPool()
    {
        if (ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator HitFlash()
    {
        foreach (var r in renderers)
        {
            if (r != null) r.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null) renderers[i].material.color = originalColors[i];
        }
    }
    
}
