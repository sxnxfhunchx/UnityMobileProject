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

        // для будущего оружия
        public string itemId;
    }
}