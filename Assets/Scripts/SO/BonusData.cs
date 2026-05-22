using UnityEngine;

[CreateAssetMenu(fileName = "NewBonusData", menuName = "ScriptableObjects/BonusData")]
public class BonusData : ScriptableObject
{
    public string bonusName;
    public int scoreValue = 25;
}
