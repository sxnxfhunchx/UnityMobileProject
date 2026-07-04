using Notifications;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private OrientationObserver menuObserver;
    [SerializeField] private GameObject questsMenuPanel;
    [SerializeField] private CharacterSelectionController selectionController;
    
    [SerializeField] private Button startBuyButton; 
    [SerializeField] private TMP_Text startBuyButtonText;
    
    [SerializeField] private Image coinIcon;
    
    [Header("New Locked Overlay Settings")]
    [SerializeField] private GameObject lockedCharacterOverlay;
    
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
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (questsMenuPanel != null) questsMenuPanel.SetActive(false);
        
        if (DailyNotificationManager.Instance != null &&
            DailyNotificationManager.Instance.ConsumeNotificationLaunch())
        {
            //OpenDailyRewardPanel();
        }
        
        if (selectionController != null)
        {
            selectionController.OnCharacterSelected += (data) => RefreshStartButton();
        }
        
        RefreshStartButton();
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
        bool isMenuInteractable = !settingsPanel.activeSelf 
                                  && !saveLoadPanel.activeSelf 
                                  && !questsMenuPanel.activeSelf;
        menuObserver.ActiveMenu.SetMenuInteractable(isMenuInteractable);
    }
    
    public void OpenQuestsPanel()
    {
        if (questsMenuPanel == null) return;
        questsMenuPanel.SetActive(true);
        SetMenuInteractable();

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
        SetMenuInteractable();
    }
    
    private void RefreshStartButton()
    {
        if (selectionController == null || startBuyButton == null) return;

        var current = selectionController.CurrentCharacter;
        bool isUnlocked = selectionController.IsUnlocked(current.characterId);

        startBuyButton.onClick.RemoveAllListeners();

        if (isUnlocked)
        {
            if (startBuyButtonText != null) startBuyButtonText.text = "START";
            if (coinIcon != null) coinIcon.gameObject.SetActive(false); 
            if (lockedCharacterOverlay != null) lockedCharacterOverlay.SetActive(false); 

            startBuyButton.onClick.AddListener(() => { 
                menuObserver.ActiveMenu.StartGame(); 
            });
        }
        else
        {
            if (startBuyButtonText != null) startBuyButtonText.text = current.characterPrice.ToString();
            if (coinIcon != null) coinIcon.gameObject.SetActive(true); 
            if (lockedCharacterOverlay != null) lockedCharacterOverlay.SetActive(true); 

            startBuyButton.onClick.AddListener(() => {
                selectionController.TryPurchaseCharacter(current.characterId);
                
                MetaUIController metaUI = FindFirstObjectByType<MetaUIController>();
                if (metaUI != null)
                {
                    metaUI.gameObject.SetActive(false);
                    metaUI.gameObject.SetActive(true);
                }

                RefreshStartButton(); 
            });
        }
    }
    public void OnAddGoldCheatButtonPressed()
    {
        if (QuestManager.Instance != null)
        {
            
            QuestManager.Instance.SpendGold(-100); 

            MetaUIController metaUI = FindFirstObjectByType<MetaUIController>();
            if (metaUI != null)
            {
                metaUI.gameObject.SetActive(false);
                metaUI.gameObject.SetActive(true);
            }
            
            RefreshStartButton();
        }
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
