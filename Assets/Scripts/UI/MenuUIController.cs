using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private OrientationObserver menuObserver;
    [SerializeField] private GameObject questsMenuPanel;
    
    void Start()
    {
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (questsMenuPanel != null) questsMenuPanel.SetActive(false);
    }

    public void OpenSaveLoadPanel()
    {
        if (saveLoadPanel == null)
            return;
        
        SetMenuInteractable(false);
        saveLoadPanel.SetActive(true);
        
        
    }
    
    public void CloseSaveLoadPanel()
    {
        if (saveLoadPanel == null)
            return;
        
        saveLoadPanel.SetActive(false);
        SetMenuInteractable(true);
    }
    
    public void OpenSettingsPanel()
    {
        if (settingsPanel == null)
            return;
        
        SetMenuInteractable(false);
        settingsPanel.SetActive(true);
    }
    
    public void CloseSettingsPanel()
    {
        if (settingsPanel == null)
            return;
        
        settingsPanel.SetActive(false);
        SetMenuInteractable(true);
    }
    
    public void SetMenuInteractable(bool value)
    {
        menuObserver.ActiveMenu.SetMenuInteractable(value);
    }
    public void OpenQuestsPanel()
    {
        if (questsMenuPanel == null) return;
        SetMenuInteractable(false);
        questsMenuPanel.SetActive(true);

        QuestMenuController controller = questsMenuPanel.GetComponent<QuestMenuController>();
        if (controller != null)
        {
            controller.RefreshQuestMenu();
        }
    }
    public void CloseQuestsPanel()
    {
        if (questsMenuPanel == null) return;
        questsMenuPanel.SetActive(false);
        SetMenuInteractable(true);
    }
    
    public void OnResetQuestsButtonPressed()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ResetAllQuestsProgress();
        
            QuestMenuController controller = FindFirstObjectByType<QuestMenuController>();
            if (controller != null)
            {
                controller.RefreshQuestMenu();
            }
        }
    }
    
}
