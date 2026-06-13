using Ability;
using Interfaces;
using UnityEngine;

namespace SO.PowerUps
{
    [CreateAssetMenu(menuName = "Power Ups/Energy Blast")]
    public class EnergyBlastPowerUpData : PowerUpData
    {
        public float Radius = 6f;
        public int Damage = 999;
        
        [Header("Audio")]
        public AudioClip BlastSound;

        public override IPlayerAbility CreateAbility(MonoBehaviour coroutineRunner)
        {
            return new EnergyBlastAbility(this, coroutineRunner);
        }
    }
}