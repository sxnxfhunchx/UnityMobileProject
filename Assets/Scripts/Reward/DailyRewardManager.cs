using System;
using SO;
using UnityEngine;

namespace Reward
{
    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        [SerializeField] private RewardData defaultDailyReward;
        [SerializeField] private DailyRewardSettings dailyRewardSettings;

        public RewardData CurrentReward => defaultDailyReward;

        private TimeSpan RewardCooldown => dailyRewardSettings.Cooldown;
        
        private void Awake()
        {
            Debug.Log("DailyRewardManager Awake");
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        public bool CanClaimReward()
        {
            if (!PlayerPrefs.HasKey(DailyRewardKeys.LastSessionTime))
                return false;

            string savedTime = PlayerPrefs.GetString(DailyRewardKeys.LastSessionTime);

            if (!DateTime.TryParse(savedTime, out DateTime lastSessionTime))
                return false;
            
            return DateTime.UtcNow - lastSessionTime >= RewardCooldown;
        }

        public void ClaimReward()
        {
            ApplyReward(defaultDailyReward);

            PlayerPrefs.SetString(DailyRewardKeys.LastSessionTime, DateTime.Now.ToString("O"));
            PlayerPrefs.Save();
        }
        
        private void ApplyReward(RewardData reward)
        {
            switch (reward.rewardType)
            {
                case RewardType.Coins:
                    // CurrencyManager.Instance.AddCoins(reward.amount);
                    Debug.Log($"Claimed coins: {reward.amount}");
                    QuestManager.Instance.GainGold(reward.amount);
                    break;

                case RewardType.Weapon:
                    // InventoryManager.Instance.AddItem(reward.itemId);
                    Debug.Log($"Claimed weapon: {reward.itemId}");
                    break;
            }
        }
    }
}