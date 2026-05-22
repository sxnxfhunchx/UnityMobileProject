using System;
using UnityEngine;

public interface IPlayerInput
{
    event Action<Vector3> OnMoveInput;
    event Action<bool> OnShootInput;
    event Action OnDashInput;
}

