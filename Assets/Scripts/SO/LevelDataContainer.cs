using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDataContainer", menuName = "ScriptableObjects/LevelDataContainer")]
public class LevelDataContainer : ScriptableObject
{
    public List<LevelSettings> levels;
}