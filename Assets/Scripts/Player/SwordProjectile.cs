using System.Collections;
using UnityEngine;

public class SwordProjectile : MonoBehaviour
{
    public float speed = 30f;
    public int damage = 1;
    public float lifeTime = 3f;
    public string poolTag = "Projectile"; 

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

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyMovement enemy = other.GetComponent<EnemyMovement>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage); 
                ReturnToPool();  
            }
        }
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