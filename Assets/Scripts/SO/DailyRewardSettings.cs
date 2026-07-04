using System;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Settings/Daily Reward Settings")]
    public class DailyRewardSettings : ScriptableObject
    {
        [Header("Production")]
        public float cooldownHours = 24;

        [Header("Testing")]
        public bool testMode;
        public float testCooldownSeconds = 30;

        public TimeSpan Cooldown =>
            testMode
                ? TimeSpan.FromSeconds(testCooldownSeconds)
                : TimeSpan.FromHours(cooldownHours);
    }
}