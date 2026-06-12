using System;

namespace Interfaces
{
    public interface IPlayerAbility
    {
        bool CanUse { get; }
        bool IsActive { get; }
        
        event Action StateChanged;

        void Use();
    }
}