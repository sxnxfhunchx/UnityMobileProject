using Ability;
using Interfaces;
using UnityEngine;

namespace SO.PowerUps
{
    [CreateAssetMenu(menuName = "Power Ups/Shield")]
    public class ShieldPowerUpData : PowerUpData
    {
        [Header("Shield Settings")]
        public int ShieldHealth = 1;

        public override IPlayerAbility CreateAbility(MonoBehaviour coroutineRunner)
        {
            return new ShieldAbility(this, coroutineRunner);
        }
    }
}