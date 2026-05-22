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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}