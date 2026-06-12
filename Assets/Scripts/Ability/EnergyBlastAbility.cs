using System.Collections;
using SO.PowerUps;
using UnityEngine;

namespace Ability
{
    public class EnergyBlastAbility : PlayerAbility
    {
        private readonly EnergyBlastPowerUpData blastData;
        private readonly Transform origin;

        private const string BlastEffectTag = "BlastVFX";
        
        public EnergyBlastAbility(EnergyBlastPowerUpData data, MonoBehaviour coroutineRunner)
            : base(data, coroutineRunner)
        {
            blastData = data;
            origin = coroutineRunner.transform;
        }

        public override void Use()
        {
            if (!CanUse)
                return;

            Blast();
            StartCooldown();
        }

        private void Blast()
        {
            ObjectPooler.Instance.SpawnFromPool(BlastEffectTag, origin.position, Quaternion.identity);
            
            Collider[] hits = Physics.OverlapSphere(origin.position, blastData.Radius);
            
            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out EnemyController enemyController))
                {
                    enemyController.TakeDamage(blastData.Damage);
                }
            }
        }
    }
}