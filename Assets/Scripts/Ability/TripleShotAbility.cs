using System;
using System.Collections;
using Interfaces;
using UnityEngine;

namespace Ability
{
    public class TripleShotAbility : MonoBehaviour, IPlayerAbility
    {
        [SerializeField] private float duration = 5f;
        [SerializeField] private float cooldown = 10f;
        
        public bool IsActive { get; private set; }
        public bool CanUse { get; private set; } = true;

        public event Action StateChanged;
        
        public void Use()
        {
            if (!CanUse)
                return;
            
            StartCoroutine(AbilityCoroutine());
        }
        
        private IEnumerator AbilityCoroutine()
        {
            CanUse = false;
            IsActive = true;
            StateChanged?.Invoke();

            yield return new WaitForSeconds(duration);

            IsActive = false;
            StateChanged?.Invoke();

            yield return new WaitForSeconds(cooldown);
            
            CanUse = true;
            StateChanged?.Invoke();
        }
    }
}