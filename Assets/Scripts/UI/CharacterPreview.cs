using SO;
using UnityEngine;

public class CharacterPreview : MonoBehaviour
{
    [SerializeField] private CharacterSelectionController controller;
    [SerializeField] private Transform previewRoot;

    private GameObject currentPreview;

    private void OnEnable()
    {
        controller.OnCharacterSelected += UpdateCharacter;
    }

    private void OnDisable()
    {
        controller.OnCharacterSelected -= UpdateCharacter;
    }

    private void UpdateCharacter(CharacterData data)
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = Instantiate(
            data.previewPrefab,
            previewRoot.position,
            previewRoot.rotation,
            previewRoot
        );
    }
}

