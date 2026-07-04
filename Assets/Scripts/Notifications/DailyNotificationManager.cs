using System;
using System.Collections;
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
            
        public static DailyNotificationManager Instance { get; private set; }
        
        private const string ChannelId = "daily_reward_channel";
        private const int DailyNotificationId = 1001;

        private PermissionRequest permissionRequest;

        public bool openedFromNotification;
        
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

            Debug.Log($"Permission: {permissionRequest.Status}");
            if (permissionRequest.Status != PermissionStatus.Allowed)
            {
                Debug.Log("Notification permission not granted.");
                yield break;
            }
            
            // Check if open from notification
            var intentData = AndroidNotificationCenter.GetLastNotificationIntent();
            openedFromNotification = intentData != null;

            CancelDailyNotification();
            RegisterAndroidChannel();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                CancelDailyNotification();
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (pause)
                ScheduleDailyNotification();
            else
                CancelDailyNotification();
        }
        
        private void OnApplicationQuit()
        {
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
            if (permissionRequest.Status != PermissionStatus.Allowed)
                return;
            
            AndroidNotificationCenter.CancelScheduledNotification(DailyNotificationId);

            var notification = new AndroidNotification
            {
                Title = Title,
                Text = Text,
                //FireTime = DateTime.Now.AddHours(24),
                FireTime = DateTime.Now.AddMinutes(1),
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
    }
}