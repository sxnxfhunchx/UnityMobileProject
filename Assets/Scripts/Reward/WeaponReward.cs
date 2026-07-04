using SO;
using UnityEngine;

namespace Reward
{
    public class WeaponReward : Reward
    {
        private readonly WeaponData weapon;

        public override Sprite Icon => weapon.Icon;
        public override string Name => weapon.WeaponName;
        
        public WeaponReward(WeaponData weapon)
        {
            this.weapon = weapon;
        }
        
        public override void Apply()
        {
            WeaponInventory.Instance.UnlockWeapon(weapon);
        }
    }
}