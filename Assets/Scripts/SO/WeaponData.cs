using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Rewards/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string WeaponId;
        public string WeaponName;
        public Sprite Icon;
        
        [Header("Visual")]
        public GameObject VisualPrefab;

        [Header("Stats")]
        public int Damage = 10;
        public float FireRate = 0.2f;
    }
}