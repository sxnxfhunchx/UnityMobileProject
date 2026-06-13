using Ability;
using SO.PowerUps;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private MeshRenderer visualRenderer;
    [SerializeField] private string poolTag = "PowerUp";
    [SerializeField] private AudioClip pickupSound;
    private PowerUpData powerUpData;

    public void Initialize(PowerUpData data)
    {
        powerUpData = data;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (powerUpData == null || powerUpData.Icon == null || visualRenderer == null)
            return;

        visualRenderer.material.mainTexture = powerUpData.Icon.texture;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (powerUpData == null)
            return;

        PlayerAbilityController abilityController = other.GetComponentInParent<PlayerAbilityController>();

        if (abilityController == null)
            return;

        abilityController.SetAbility(powerUpData);
        SoundManager.Instance?.PlaySound(pickupSound, transform.position);

        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}
