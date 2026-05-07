using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public SoundData soundData; 
    private int currentHealth;

    private Renderer[] renderers;
    private Color[] originalColors;

    void Start()
    {
        if (data != null) currentHealth = data.health;

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    void Update()
    {
        float moveSpeed = (data != null) ? data.speed : 15f;
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Get random sound from ScriptableObject and play via SoundManager
        if (SoundManager.Instance != null && soundData != null)
        {
            AudioClip clip = soundData.GetRandomHitSound();
            SoundManager.Instance.PlaySound(clip, transform.position);
        }

        StartCoroutine(HitFlash());

        if (currentHealth <= 0) Die();
    }
    
    public void ScaleBoss(int factor)
    {
        currentHealth *= factor;
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

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(data.scoreValue);
        }

        Destroy(gameObject);
    }
}