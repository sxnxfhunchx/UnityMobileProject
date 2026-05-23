using System.Collections;
using UnityEngine;

public class SwordProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private string poolTag = "Projectile"; 

    private Coroutine deactivateCoroutine;

    void OnEnable()
    {
        deactivateCoroutine = StartCoroutine(DeactivateAfterTime());
    }

    void OnDisable()
    {
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
    }
    
    void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime), Space.World);
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;
        
        if (!other.TryGetComponent(out EnemyController enemy))
            return;
        
        enemy.TakeDamage(damage); 
        ReturnToPool();  
    }

    void ReturnToPool()
    {
        if (ObjectPooler.Instance == null)
            return;
        
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}