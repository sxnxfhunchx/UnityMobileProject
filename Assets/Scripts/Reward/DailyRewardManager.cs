using SO;
using UnityEngine;

namespace Reward
{
    public class DailyRewardManager
    {
        [SerializeField] private RewardData dailyReward;

        public RewardData CurrentReward => dailyReward;

        public void ClaimReward()
        {
            switch (dailyReward.rewardType)
            {
                case RewardType.Coins:
                    // TODO: implement
                    break;

                case RewardType.Weapon:
                    // TODO: implement
                    break;
            }
        }
    }
}