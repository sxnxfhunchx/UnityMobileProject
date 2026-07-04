using UnityEngine;

namespace Reward
{
    public abstract class Reward
    {
        public abstract Sprite Icon { get; }
        public abstract string Name { get; }

        public abstract void Apply();
    }
}