using System;
using System.Collections;
using Interfaces;
using SO.PowerUps;
using UnityEngine;

namespace Ability
{
    public class TripleShotAbility : PlayerAbility
    {
        private readonly TripleShotPowerUpData tripleShotData;

        public int ProjectileCount => tripleShotData.ProjectileCount;
        public float SpreadAngle => tripleShotData.SpreadAngle;

        public TripleShotAbility(TripleShotPowerUpData data, MonoBehaviour coroutineRunner)
            : base(data, coroutineRunner)
        {
            tripleShotData = data;
        }
        
        public override void Use()
        {
            if (!CanUse)
                return;

            coroutineRunner.StartCoroutine(DurationCoroutine());
        }

        private IEnumerator DurationCoroutine()
        {
            CanUse = false;
            IsActive = true;
            RaiseStateChanged();

            yield return new WaitForSeconds(data.Duration);

            StartCooldown();
        }
    }
}