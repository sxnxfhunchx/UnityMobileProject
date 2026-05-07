using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        PlayerMovementNew movement = GetComponent<PlayerMovementNew>();
        if (movement != null) movement.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Test");
            TakeDamage(10);
            
            Destroy(other.gameObject); 
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}