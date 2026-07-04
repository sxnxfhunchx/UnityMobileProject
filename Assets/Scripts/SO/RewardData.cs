using Reward;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Rewards/Daily Reward")]
    public class RewardData : ScriptableObject
    {
        public RewardType rewardType;
        public string rewardName;
        public Sprite icon;
        public int amount;
        
        public string itemId;

        public string GetInfo()
        {
            switch (rewardType)
            {
                case RewardType.Coins:
                    return $"{amount} {rewardName}";
                case RewardType.Weapon:
                    return rewardName;
            }
            return "";
        }
    }
}