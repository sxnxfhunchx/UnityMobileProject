using Unity.Services.Analytics;

namespace Analytics
{
    public class DailyRewardClaimedEvent : Event
    {
        public DailyRewardClaimedEvent() : base("daily_reward_claimed")
        {
        }
        
        public string RewardName
        {
            set => SetParameter("daily_reward", value);
        }
    }
}