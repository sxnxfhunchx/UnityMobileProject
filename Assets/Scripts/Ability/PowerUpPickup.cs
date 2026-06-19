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

        if (TryGetComponent(out PooledObjectMovement movement))
        {
            string uniqueTag = string.IsNullOrEmpty(data.poolTag) ? data.name : data.poolTag;
            movement.SetPoolTag(uniqueTag);
        }
    }

    private void UpdateVisual()
    {
        if (powerUpData == null || powerUpData.PickupTexture == null || visualRenderer == null)
            return;

        visualRenderer.material.SetTexture("_BaseMap", powerUpData.PickupTexture);
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

        string currentTag = string.IsNullOrEmpty(powerUpData.poolTag) ? powerUpData.name : powerUpData.poolTag;
        ObjectPooler.Instance.ReturnToPool(currentTag, gameObject);    }
}
