using Ability;
using Interfaces;
using UnityEngine;

namespace SO.PowerUps
{
    [CreateAssetMenu(menuName = "Power Ups/Triple Shot")]
    public class TripleShotPowerUpData : PowerUpData
    {
        [Header("Triple Shot Settings")]
        public int ProjectileCount = 3;
        public float SpreadAngle = 15f;
        
        public override IPlayerAbility CreateAbility(MonoBehaviour coroutineRunner)
        {
            return new TripleShotAbility(this, coroutineRunner);
        }
    }
}