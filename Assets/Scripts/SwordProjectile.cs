using UnityEngine;

public class SwordProjectile : MonoBehaviour
{
    public float speed = 30f;
    public int damage = 1;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            
            Debug.Log("Попал в: " + other.name);
            EnemyController enemy = other.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage); 
                Destroy(gameObject);      
            }
        }
    }
}