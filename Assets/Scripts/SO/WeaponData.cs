using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Rewards/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string WeaponId;
        public string WeaponName;
        public Sprite Icon;

        [Header("Future gameplay stats")]
        public int damage = 1;
        public float cooldown = 0.5f;
    }
}