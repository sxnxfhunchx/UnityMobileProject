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

        if (index < 0)
            index = characterDatabase.Characters.Length - 1;

        if (index >= characterDatabase.Characters.Length)
            index = 0;

        currentIndex = index;

        OnCharacterSelected?.Invoke(CurrentCharacter);
    }
    
    
}
