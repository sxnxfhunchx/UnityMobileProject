using System;
using System.Collections;
using Interfaces;
using UnityEngine;

namespace Ability
{
    public class PlayerAbilityController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour defaultAbilitySource;
        [SerializeField] private MonoBehaviour inputSource;
        
        private IPlayerAbility currentAbility;
        
        public event Action<bool> OnAbilityAvailabilityChanged;

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
            if (defaultAbilitySource is IPlayerAbility ability)
            {
                SetAbility(ability);
            }
        }
        
        public void SetAbility(IPlayerAbility ability)
        {
            if (currentAbility != null)
                currentAbility.StateChanged -= NotifyAvailability;

            currentAbility = ability;

            if (currentAbility != null)
                currentAbility.StateChanged += NotifyAvailability;

            NotifyAvailability();
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
    }
}