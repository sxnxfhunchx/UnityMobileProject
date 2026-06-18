using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public string poolTag;
    
    [Header("Base Stats")]
    public int health = 1;
    public float speed = 15f;
    public int damage = 10;
    public int scoreValue = 10;

    [Header("Tier / Weapon Modifiers")]
    public float rotationSpeedY = 180f; 
    public float fireRateModifier = 1.0f; 
    
    [Header("Status")]
    public bool isBerserkMode = false; 
}