using Ability;
using UnityEngine;

public class ShieldView : MonoBehaviour
{
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private GameObject shieldView;
    [SerializeField] private float rotationSpeed = 60f;
    
    private ShieldAbility currentShield;
    
    private void OnEnable()
    {
        abilityController.OnAbilityChanged += SubscribeToShield;
        SubscribeToShield();
    }

    private void OnDisable()
    {
        abilityController.OnAbilityChanged -= SubscribeToShield;
        UnsubscribeFromShield();
    }
    
    private void Update()
    {
        if (shieldView.activeSelf)
            shieldView.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void SubscribeToShield()
    {
        UnsubscribeFromShield();

        if (!abilityController.TryGetCurrentAbility(out currentShield))
        {
            shieldView.SetActive(false);
            return;
        }

        currentShield.StateChanged += UpdateView;
        UpdateView();
    }

    private void UnsubscribeFromShield()
    {
        if (currentShield != null)
            currentShield.StateChanged -= UpdateView;

        currentShield = null;
    }

    private void UpdateView()
    {
        shieldView.SetActive(currentShield != null && currentShield.IsActive);
    }
}
