using System;
using System.Collections;
using Interfaces;
using SO.PowerUps;
using UnityEngine;

namespace Ability
{
    public class PlayerAbilityController : MonoBehaviour
    {
        [SerializeField] private PowerUpData defaultPowerUp;
        
        [SerializeField] private MonoBehaviour inputSource;
        
        private IPlayerAbility currentAbility;
        
        public event Action<bool> OnAbilityAvailabilityChanged;
        public event Action OnAbilityChanged;

        private IPlayerInput playerInput;

        private void OnEnable()
        {
            playerInput = inputSource as IPlayerInput;

            if (playerInput == null)
                return;

            playerInput.OnAbilityInput += UseCurrentAbility;
        }

        private void OnDisable()
        {
            if (playerInput == null)
                return;

            playerInput.OnAbilityInput -= UseCurrentAbility;
        }
        
        private void Awake()
        {
            if (defaultPowerUp != null)
                SetAbility(defaultPowerUp);
        }
        
        private void Start()
        {
            if (defaultPowerUp != null)
                SetAbility(defaultPowerUp);
        }
        
        public void SetAbility(PowerUpData powerUpData)
        {
            if (currentAbility != null)
                currentAbility.StateChanged -= NotifyAvailability;

            currentAbility = powerUpData.CreateAbility(this);

            if (currentAbility != null)
                currentAbility.StateChanged += NotifyAvailability;

            NotifyAvailability();
            OnAbilityChanged?.Invoke();
        }
        
        public void UseCurrentAbility()
        {
            if (currentAbility == null)
                return;

            if (!currentAbility.CanUse)
                return;

            currentAbility.Use();
            NotifyAvailability();
        }
        
        private void NotifyAvailability()
        {
            bool canUse = currentAbility != null && currentAbility.CanUse;
            OnAbilityAvailabilityChanged?.Invoke(canUse);
        }
        
        public bool TryGetCurrentAbility<T>(out T ability) where T : class
        {
            ability = currentAbility as T;
            return ability != null;
        }
    }
}