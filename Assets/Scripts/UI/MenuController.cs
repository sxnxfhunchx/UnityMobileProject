using SO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private CharacterSelectionController controller;
    
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text HealthText;
    
    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    
    [SerializeField] private TouchObserver touchObserver;
    
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
    
    public void SetMenuInteractable(bool value)
    {
        startButton.interactable = value;
        loadButton.interactable = value;
        settingsButton.interactable = value;
        exitButton.interactable = value;

        leftButton.interactable = value;
        rightButton.interactable = value;

        touchObserver.enabled = value;
    }
}
