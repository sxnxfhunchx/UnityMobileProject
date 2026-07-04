using System;
using SO;
using UnityEngine;

namespace Reward
{
    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        private const string LastSessionTicksKey = "DailyReward_LastSessionTicks";
        
        [SerializeField] private RewardData defaultDailyReward;
        [SerializeField] private DailyRewardSettings dailyRewardSettings;

        public RewardData CurrentReward => defaultDailyReward;

        private TimeSpan RewardCooldown => dailyRewardSettings.Cooldown;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        public bool CanClaimReward()
        {
            if (!TryGetLastSessionTime(out DateTime lastSessionTime))
                return false;

            TimeSpan timePassed = DateTime.UtcNow - lastSessionTime;

            return timePassed >= RewardCooldown;
        }

        public void ClaimReward()
        {
            ApplyReward(defaultDailyReward);
        }
        
        private void ApplyReward(RewardData reward)
        {
            switch (reward.rewardType)
            {
                case RewardType.Coins:
                    QuestManager.Instance.GainGold(reward.amount);
                    break;

                case RewardType.Weapon:
                    // InventoryManager.Instance.AddItem(reward.itemId);
                    Debug.Log($"Claimed weapon: {reward.itemId}");
                    break;
            }
        }
        
        public void UpdateLastSessionTime()
        {
            PlayerPrefs.SetString(LastSessionTicksKey, DateTime.UtcNow.Ticks.ToString());
            PlayerPrefs.Save();
        }

        private bool TryGetLastSessionTime(out DateTime lastSessionTime)
        {
            lastSessionTime = default;

            string savedTicks = PlayerPrefs.GetString(LastSessionTicksKey, "");

            if (!long.TryParse(savedTicks, out long ticks))
                return false;

            lastSessionTime = new DateTime(ticks, DateTimeKind.Utc);
            return true;
        }
    }
}