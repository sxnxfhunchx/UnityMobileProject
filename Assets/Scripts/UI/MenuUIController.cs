using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private OrientationObserver menuObserver;
    
    private void OnEnable()
    {
        menuObserver.OnOrientationChanged += SetMenuInteractable;
    }

    private void OnDisable()
    {
        menuObserver.OnOrientationChanged -= SetMenuInteractable;
    }
    
    void Start()
    {
        saveLoadPanel.SetActive(false);
        settingsPanel.SetActive(false);
        SetMenuInteractable();
    }

    public void OpenSaveLoadPanel()
    {
        if (saveLoadPanel == null)
            return;
        
        saveLoadPanel.SetActive(true);
        SetMenuInteractable();
    }
    
    public void CloseSaveLoadPanel()
    {
        if (saveLoadPanel == null)
            return;
        
        saveLoadPanel.SetActive(false);
        SetMenuInteractable();
    }
    
    public void OpenSettingsPanel()
    {
        if (settingsPanel == null)
            return;
        
        settingsPanel.SetActive(true);
        SetMenuInteractable();
    }
    
    public void CloseSettingsPanel()
    {
        if (settingsPanel == null)
            return;
        
        settingsPanel.SetActive(false);
        SetMenuInteractable();
    }
    
    public void SetMenuInteractable()
    {
        bool isMenuInteractable = !settingsPanel.activeSelf && !saveLoadPanel.activeSelf;
        menuObserver.ActiveMenu.SetMenuInteractable(isMenuInteractable);
    }
}
