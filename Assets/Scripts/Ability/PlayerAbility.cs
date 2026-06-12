using System;
using System.Collections;
using Interfaces;
using SO.PowerUps;
using UnityEngine;

namespace Ability
{
    public abstract class PlayerAbility : IPlayerAbility
    {
        protected readonly PowerUpData data;
        protected readonly MonoBehaviour coroutineRunner;

        public bool CanUse { get; protected set; } = true;
        public bool IsActive { get; protected set; }

        public event Action StateChanged;

        protected PlayerAbility(PowerUpData data, MonoBehaviour coroutineRunner)
        {
            this.data = data;
            this.coroutineRunner = coroutineRunner;
        }

        public abstract void Use();
        
        protected IEnumerator CooldownCoroutine()
        {
            yield return new WaitForSeconds(data.Cooldown);

            CanUse = true;
            RaiseStateChanged();
        }

        protected void StartCooldown()
        {
            IsActive = false;
            CanUse = false;
            RaiseStateChanged();

            coroutineRunner.StartCoroutine(CooldownCoroutine());
        }

        protected void RaiseStateChanged()
        {
            Debug.Log("Raise StateChanged");
            StateChanged?.Invoke();
        }
    }
}