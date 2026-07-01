using System;
using SO;
using UnityEngine;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] private CharactersDatabase characterDatabase;
    private int currentIndex;

    public event Action<CharacterData> OnCharacterSelected;

    public CharacterData CurrentCharacter => characterDatabase.Characters[currentIndex];

    private void Start()
    {
        QuestManager.Instance.UnlockCharacter("0"); 
        SelectCharacter(0);
    }

    public void SelectNext()
    {
        SelectCharacter(currentIndex + 1);
    }

    public void SelectPrevious()
    {
        SelectCharacter(currentIndex - 1);
    }

    private void SelectCharacter(int index)
    {
        if (characterDatabase == null || characterDatabase.Characters == null || characterDatabase.Characters.Length == 0)
            return;

        if (index < 0) index = characterDatabase.Characters.Length - 1;
        if (index >= characterDatabase.Characters.Length) index = 0;

        currentIndex = index;

        OnCharacterSelected?.Invoke(CurrentCharacter);
    }
    
    public bool IsUnlocked(string characterId)
    {
        return QuestManager.Instance.IsUnlocked(characterId); 
    }
    
    public void TryPurchaseCharacter(string characterId)
    {
        if (QuestManager.Instance.GetTotalGold() >= 50)
        {
            QuestManager.Instance.SpendGold(50);
            QuestManager.Instance.UnlockCharacter(characterId);
        }
    }
    
    
}
