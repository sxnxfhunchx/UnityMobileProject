using System;
using System.Collections;
using Interfaces;
using SO.PowerUps;
using UnityEngine;

namespace Ability
{
    public class ShieldAbility : PlayerAbility
    {
        private readonly ShieldPowerUpData shieldData;
        private int shieldCharges;

        public ShieldAbility(ShieldPowerUpData data, MonoBehaviour coroutineRunner)
            : base(data, coroutineRunner)
        {
            shieldData = data;
        }
        
        public override void Use()
        {
            if (!CanUse)
                return;

            shieldCharges = shieldData.ShieldHealth;

            CanUse = false;
            IsActive = true;
            RaiseStateChanged();
        }

        public bool TryBlockDamage()
        {
            if (!IsActive)
                return false;

            shieldCharges--;

            if (shieldCharges <= 0)
                StartCooldown();
            else
                RaiseStateChanged();

            return true;
        }

    }
}