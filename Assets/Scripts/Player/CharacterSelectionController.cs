using System;
using SO;
using UnityEngine;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] private CharacterData[] characters;

    private int currentIndex;

    public event Action<CharacterData> OnCharacterSelected;

    public CharacterData CurrentCharacter => characters[currentIndex];

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
        if (characters == null || characters.Length == 0)
            return;

        if (index < 0)
            index = characters.Length - 1;

        if (index >= characters.Length)
            index = 0;

        currentIndex = index;

        OnCharacterSelected?.Invoke(CurrentCharacter);
    }
}
