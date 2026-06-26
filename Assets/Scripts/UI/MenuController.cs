using SO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private CharacterSelectionController controller;
    
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text HealthText;
    
    private void OnEnable()
    {
        controller.OnCharacterSelected += UpdateCharacter;
    }

    private void OnDisable()
    {
        controller.OnCharacterSelected -= UpdateCharacter;
    }

    public void StartGame()
    {
        GameManager.Instance.SetSelectedCharacter(controller.CurrentCharacter);
        SceneManager.LoadScene(1);
    }
    
    public void LoadGame()
    {
        // TODO:
        Debug.Log("Load Game to be implemented");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    private void UpdateCharacter(CharacterData data)
    {
        nameText.text = data.characterName;
        speedText.text = $"Speed: {data.speed}";
        HealthText.text = $"Health: {data.health}";
    }
}
