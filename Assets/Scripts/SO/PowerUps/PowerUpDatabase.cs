using UnityEngine;

namespace SO.PowerUps
{
    [CreateAssetMenu(fileName = "PowerUpDatabase", menuName = "ScriptableObjects/PowerUpDatabase", order = 0)]
    public class PowerUpDatabase : ScriptableObject
    {
        [SerializeField] private PowerUpData[] powerUps;

        public PowerUpData[] PowerUps => powerUps;
        
        public PowerUpData GetById(string id)
        {
            foreach (PowerUpData powerUp in powerUps)
            {
                if (powerUp.poolTag == id)
                    return powerUp;
            }

            return null;
        }
        
        public PowerUpData GetDefault()
        {
            return null;
        }
        
    }
}