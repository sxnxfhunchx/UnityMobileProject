using Interfaces;
using UnityEngine;

namespace SO.PowerUps
{
    public abstract class PowerUpData : ScriptableObject
    {
        [Header("Base Settings")]
        public string DisplayName;
        public string poolTag;
        public Sprite Icon;
        public Texture2D PickupTexture;

        public float Duration = 5f;
        public float Cooldown = 10f;
        
        [Header("Audio")]
        public AudioClip BlockSound;
        
        public abstract IPlayerAbility CreateAbility(MonoBehaviour coroutineRunner);
    }
}