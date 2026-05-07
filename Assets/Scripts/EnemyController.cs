using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data; 
    private int currentHealth;

    void Start()
    {
        if (data != null)
        {
            currentHealth = data.health;
        }
    }

    void Update()
    {
        float moveSpeed = (data != null) ? data.speed : 15f;
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
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