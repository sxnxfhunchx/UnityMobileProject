using System.Collections;
using Ability;
using SO.PowerUps;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonView : MonoBehaviour
{
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    
    private Coroutine cooldownCoroutine;
    
    private void OnEnable()
    {
        abilityController.OnAbilityChanged += UpdateIcon;
        abilityController.OnAbilityCooldownStarted += StartCooldownView;
    }

    private void OnDisable()
    {
        abilityController.OnAbilityChanged -= UpdateIcon;
        abilityController.OnAbilityCooldownStarted -= StartCooldownView;
    }
    
    private void UpdateIcon()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        PowerUpData data = abilityController.CurrentPowerUpData;
        
        if (data != null && data.Icon != null)
        {
            iconImage.sprite = data.Icon;
            button.interactable = true;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            button.interactable = false;
        }
        cooldownOverlay.fillAmount = 0f;
    }
    
    private void StartCooldownView(float duration)
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        cooldownCoroutine = StartCoroutine(CooldownViewCoroutine(duration));
    }

    private IEnumerator CooldownViewCoroutine(float duration)
    {
        cooldownOverlay.fillAmount = 1f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            cooldownOverlay.fillAmount = 1f - timer / duration;
            yield return null;
        }

        cooldownOverlay.fillAmount = 0f;
    }
}
