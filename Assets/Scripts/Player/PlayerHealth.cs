using Ability;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    
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
        if (abilityController.TryGetCurrentAbility<ShieldAbility>(out var shield)
            && shield.TryBlockDamage())
        {
            return;
        }
        
        currentHealth -= damage;
        SoundManager.Instance?.PlaySound(damageSound, transform.position);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SoundManager.Instance?.PlaySound(deathSound, transform.position);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
    }

}