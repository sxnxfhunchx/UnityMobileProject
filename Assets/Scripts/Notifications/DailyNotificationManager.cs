using System;
using System.Collections;
using Reward;
using SO;
using Unity.Notifications.Android;
using UnityEngine;

namespace Notifications
{
    public class DailyNotificationManager : MonoBehaviour
    {
        [Header("Daily Notification Settings")] 
        [SerializeField]
        private string Title = "Daily reward is ready!";
        [SerializeField] 
        private string Text = "Come back and claim your reward.";
        
        [SerializeField] private DailyRewardSettings dailyRewardSettings;
        
        public static DailyNotificationManager Instance { get; private set; }
        
        private const string ChannelId = "daily_reward_channel";
        private const int DailyNotificationId = 1001;

        private PermissionRequest permissionRequest;
        private bool openedFromNotification;
        private bool permissionReady;
        
        private TimeSpan NotificationDelay => dailyRewardSettings.Cooldown;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        private IEnumerator Start()
        {
            permissionRequest = new PermissionRequest();

            while (permissionRequest.Status == PermissionStatus.RequestPending)
                yield return null;
            
            if (permissionRequest.Status != PermissionStatus.Allowed)
            {
                yield break;
            }
            
            permissionReady = true;
            
            RegisterAndroidChannel();
            
            // Check if open from notification
            var intentData = AndroidNotificationCenter.GetLastNotificationIntent();
            openedFromNotification = intentData != null;

            CancelDailyNotification();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                CancelDailyNotification();
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveLastSessionTime();
                ScheduleDailyNotification();
            }
            else
            {
                CancelDailyNotification();
            }
        }
        
        private void OnApplicationQuit()
        {
            SaveLastSessionTime();
            ScheduleDailyNotification();
        }
        
        private void RegisterAndroidChannel()
        {
            var channel = new AndroidNotificationChannel
            {
                Id = ChannelId,
                Name = "Daily Rewards",
                Importance = Importance.Default,
                Description = "Daily reward reminders"
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        public void ScheduleDailyNotification()
        {
            if (!permissionReady)
                return;
            
            CancelDailyNotification();

            var notification = new AndroidNotification
            {
                Title = Title,
                Text = Text,
                FireTime = DateTime.Now + NotificationDelay,
                SmallIcon = "default",
                LargeIcon = "default"
            };

            AndroidNotificationCenter.SendNotificationWithExplicitID(
                notification,
                ChannelId,
                DailyNotificationId
            );

            Debug.Log("Daily notification scheduled.");
        }
        
        private void CancelDailyNotification()
        {
            AndroidNotificationCenter.CancelScheduledNotification(DailyNotificationId);
            AndroidNotificationCenter.CancelDisplayedNotification(DailyNotificationId);
        }
        
        public bool ConsumeNotificationLaunch()
        {
            if (!openedFromNotification)
                return false;

            openedFromNotification = false;
            return true;
        }
        
        private void SaveLastSessionTime()
        {
            DailyRewardManager.Instance?.UpdateLastSessionTime();
            PlayerPrefs.Save();
        }
    }
}