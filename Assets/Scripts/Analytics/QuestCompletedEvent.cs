using Unity.Services.Analytics;

namespace Analytics
{
    public class QuestCompletedEvent : Event
    {
        public QuestCompletedEvent() : base("quest_completed")
        {
        }
        
        public string QuestType
        {
            set => SetParameter("quest_type", value);
        }
    }
}