using System;
using System.Collections.Generic;
using SO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Reward
{
    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private DailyRewardSettings settings;

        [Header("Coins")]
        [SerializeField] private Sprite coinIcon;
        [SerializeField] private int minCoins = 50;
        [SerializeField] private int maxCoins = 150;

        [Header("Weapons")]
        [SerializeField] private WeaponDatabase possibleWeaponRewards;
        [SerializeField, Range(0f, 1f)] private float weaponRewardChance = 0.3f;

        private const string LastSessionTicksKey = "DailyReward_LastSessionTicks";

        private Reward currentReward;

        public Reward CurrentReward
        {
            get
            {
                if (currentReward == null)
                    currentReward = GenerateReward();

                return currentReward;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void UpdateLastSessionTime()
        {
            PlayerPrefs.SetString(LastSessionTicksKey,  DateTime.UtcNow.Ticks.ToString());
            PlayerPrefs.Save();
        }

        public bool CanClaimReward()
        {
            if (!TryGetLastSessionTime(out DateTime lastSessionTime))
                return false;

            TimeSpan timePassed = DateTime.UtcNow - lastSessionTime;

            return timePassed >= settings.Cooldown;
        }

        public string ClaimReward()
        {
            CurrentReward.Apply();
            currentReward = GenerateReward();
            return currentReward.Name;
        }

        private Reward GenerateReward()
        {
            WeaponData weapon = GetRandomLockedWeapon();
            
            if (weapon != null && Random.value <= weaponRewardChance)
                return new WeaponReward(weapon);

            int coins = Random.Range(minCoins, maxCoins + 1);
            return new CoinReward(coins, coinIcon);
        }

        private WeaponData GetRandomLockedWeapon()
        {
            if (possibleWeaponRewards == null || possibleWeaponRewards.Weapons.Count == 0)
                return null;

            List<WeaponData> lockedWeapons = new();

            foreach (WeaponData weapon in possibleWeaponRewards.Weapons)
            {
                if (weapon == null)
                    continue;

                if (!WeaponInventory.Instance.IsUnlocked(weapon))
                    lockedWeapons.Add(weapon);
            }

            if (lockedWeapons.Count == 0)
                return null;

            int index = Random.Range(0, lockedWeapons.Count);
            return lockedWeapons[index];
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