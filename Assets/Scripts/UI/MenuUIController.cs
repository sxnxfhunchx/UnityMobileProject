using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private OrientationObserver menuObserver;
    
    void Start()
    {
        saveLoadPanel.SetActive(false);
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
}
