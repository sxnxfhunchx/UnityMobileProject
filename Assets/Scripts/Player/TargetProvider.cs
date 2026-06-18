using System;
using Interfaces;
using UnityEngine;

public class TargetProvider : MonoBehaviour, ITargetProvider
{
    public Transform Target => transform;
}
