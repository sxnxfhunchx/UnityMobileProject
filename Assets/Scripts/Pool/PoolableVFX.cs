using UnityEngine;

public class PoolableVFX : MonoBehaviour
{
    public string poolTag = "DeathVFX";
    public float effectDuration = 0.6f; 
    private ParticleSystem particles;

    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (particles != null)
        {
            particles.Clear(); 
            particles.Play();  
        }

        Invoke("ReturnToPool", effectDuration);
    }

    void OnDisable()
    {
        CancelInvoke();  
    }

    void ReturnToPool()
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
}