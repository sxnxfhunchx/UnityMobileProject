using Unity.Services.Analytics;

namespace Analytics
{
    public class QuestRewardClaimedEvent : Event
    {
        public QuestRewardClaimedEvent() : base("quest_reward_claimed")
        {
        }
        
        public string QuestType
        {
            set => SetParameter("quest_type", value);
        }
    }
}