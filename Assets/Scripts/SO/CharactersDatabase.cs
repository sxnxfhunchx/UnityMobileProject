using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "ScriptableObjects/CharacterDatabase", order = 0)]
    public class CharactersDatabase : ScriptableObject
    {
        [SerializeField] private CharacterData[] characters;

        public CharacterData[] Characters => characters;
        
        public CharacterData GetById(string id)
        {
            foreach (CharacterData character in characters)
            {
                if (character.characterId == id)
                    return character;
            }

            return null;
        }
        
        public CharacterData GetDefault()
        {
            return characters != null && characters.Length > 0 ? characters[0] : null;
        }
    }
}