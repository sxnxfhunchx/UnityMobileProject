using Unity.Services.Analytics;

namespace Analytics
{
    public class WeaponEquippedEvent : Event
    {
        public WeaponEquippedEvent() : base("weapon_equipped")
        {
        }
        
        public string WeaponName
        {
            set => SetParameter("weapon_name", value);
        }
    }
}