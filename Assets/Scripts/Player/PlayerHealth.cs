using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    
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

}