using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private OrientationObserver menuObserver;
    
    void Start()
    {
        saveLoadPanel.SetActive(false);
    }

    public void OpenSaveLoadPanel()
    {
        SetMenuInteractable(false);
        saveLoadPanel.SetActive(true);
    }
    
    public void CloseSaveLoadPanel()
    {
        saveLoadPanel.SetActive(false);
        SetMenuInteractable(true);
    }
    
    public void SetMenuInteractable(bool value)
    {
        menuObserver.ActiveMenu.SetMenuInteractable(value);
    }
}
