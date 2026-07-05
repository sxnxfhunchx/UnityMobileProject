using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Rewards/Weapon Database")]
    public class WeaponDatabase : ScriptableObject
    {
        public List<WeaponData> Weapons = new();
        
        public WeaponData GetWeaponById(string weaponId)
        {
            foreach (WeaponData weapon in Weapons)
            {
                if (weapon != null && weapon.WeaponId == weaponId)
                    return weapon;
            }

            return null;
        }
    }
}