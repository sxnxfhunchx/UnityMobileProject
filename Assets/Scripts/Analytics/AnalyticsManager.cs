using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using Event = Unity.Services.Analytics.Event;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
    
        public static AnalyticsManager Instance { get; private set; }
    
        private bool isReady = false;

        private async void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            await Initialize();
        }

        private async Task Initialize()
        {
            await UnityServices.InitializeAsync();
        
            AnalyticsService.Instance.StartDataCollection();
        
        
            isReady = true;
            Debug.Log("Analytics Manager Initialized");

        }

        public void SendQuestRewardClaimed(string questType)
        {
            QuestRewardClaimedEvent newEvent = new QuestRewardClaimedEvent();
            newEvent.QuestType = questType;
            SendEvent(newEvent);
        }
    
        public void SendQuestCompleted(string questType)
        {
            QuestCompletedEvent newEvent = new QuestCompletedEvent();
            newEvent.QuestType = questType;
            SendEvent(newEvent);
        }
    
        public void SendWeaponEquipped(string weaponName)
        {
            WeaponEquippedEvent newEvent = new WeaponEquippedEvent();
            newEvent.WeaponName = weaponName;
            SendEvent(newEvent);
        }
    
        public void SendDailyRewardClaimed(string rewardName)
        {
            DailyRewardClaimedEvent newEvent = new DailyRewardClaimedEvent();
            newEvent.RewardName = rewardName;
            SendEvent(newEvent);
        }

        public void SendEvent(Event analyticsEvent)
        {
            if (!isReady)
                return;

            if (analyticsEvent == null)
                return;
   
            AnalyticsService.Instance.RecordEvent(analyticsEvent);
            Debug.Log($"Analytics event: {analyticsEvent}");
        }
    
        public void SendEvent(string eventName)
        {
            if (!isReady)
                return;

            AnalyticsService.Instance.RecordEvent(eventName);
            Debug.Log($"Analytics event: {eventName}");
        }
    }
}
