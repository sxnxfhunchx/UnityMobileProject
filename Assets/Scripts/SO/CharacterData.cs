using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "ScriptableObjects/CharacterData", order = 0)]
    public class CharacterData : ScriptableObject
    {
        public string characterId;
        public string characterName;
        public GameObject previewPrefab;
        public GameObject gameplayPrefab;

        [Header("Stats")]
        public float speed;
        public int health;
        
        [Header("Shop Settings")]
        public int characterPrice = 50;
    }
}