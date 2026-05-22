using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health = 1;
    public float speed = 15f;
    public int scoreValue = 10;
    public int damage = 10;
}