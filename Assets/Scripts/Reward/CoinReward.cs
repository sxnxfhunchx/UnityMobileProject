using UnityEngine;

namespace Reward
{
    public class CoinReward : Reward
    {
        
        private readonly Sprite icon;

        public int Amount { get; }

        public CoinReward(int amount, Sprite icon)
        {
            Amount = amount;
            this.icon = icon;
        }

        public override Sprite Icon => icon;

        public override string Name => $"{Amount} Gold";
  
        public override void Apply()
        {
            QuestManager.Instance.GainGold(Amount);
        }
    }
}