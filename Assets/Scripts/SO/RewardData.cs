using Reward;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Rewards/Daily Reward")]
    public class RewardData : ScriptableObject
    {
        public RewardType rewardType;
        
        [Header("Display")]
        public string rewardName;
        public Sprite icon;

        [Header("Coins")]
        public int coinsAmount;

        [Header("Weapon")]
        public WeaponData weaponReward;
        
        public string GetInfo()
        {
            switch (rewardType)
            {
                case RewardType.Coins:
                    return $"{coinsAmount} {rewardName}";
                case RewardType.Weapon:
                    return rewardName;
            }
            return "";
        }
    }
}