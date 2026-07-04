using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Rewards/Weapon Database")]
    public class WeaponDatabase : ScriptableObject
    {
        public WeaponData[] Weapons;
    }
}