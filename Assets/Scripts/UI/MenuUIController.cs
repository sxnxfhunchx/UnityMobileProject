using System.Collections;
using System.Collections.Generic;
using Notifications;
using Reward;
using SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [Header("Menu Elements")]
    [SerializeField] private Button startBuyButton; 
    [SerializeField] private TMP_Text startBuyButtonText;
    [SerializeField] private Image coinIcon;
    
    [Header("Panels")]
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private GameObject questsMenuPanel;
    [SerializeField] private GameObject weaponsPanel;
    
    [Header("Reward Panel")]
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Image rewardImage;
    
    [Header("Controllers")]
    [SerializeField] private OrientationObserver menuObserver;
    [SerializeField] private CharacterSelectionController selectionController;
    
    //[Header("New Locked Overlay Settings")]
    //[SerializeField] private GameObject lockedCharacterOverlay;
    
    private void OnEnable()
    {
        menuObserver.OnOrientationChanged += SetMenuInteractable;
    }

    private void OnDisable()
    {
        menuObserver.OnOrientationChanged -= SetMenuInteractable;
    }
    
    private IEnumerator Start()
    {
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (questsMenuPanel != null) questsMenuPanel.SetActive(false);
        if (weaponsPanel != null) weaponsPanel.SetActive(false);
        
        yield return new WaitUntil(() =>
            DailyNotificationManager.Instance != null &&
            DailyRewardManager.Instance != null
        );

        bool openedFromNotification = //true;
            DailyNotificationManager.Instance != null &&
            DailyNotificationManager.Instance.ConsumeNotificationLaunch();

        bool canClaim = //true;
            DailyRewardManager.Instance != null &&
            DailyRewardManager.Instance.CanClaimReward();

        if (openedFromNotification && canClaim)
        {
            OpenRewardPanel();
        }
        else
        {
            CloseRewardsPanel();
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

    public void OpenRewardPanel()
    {
        if (rewardPanel == null)
            return;

        Reward.Reward rewardData = DailyRewardManager.Instance.CurrentReward;
        rewardText.text = rewardData.Name;
        rewardImage.sprite = rewardData.Icon;
        rewardPanel.SetActive(true);
    }
    
    public void CloseRewardsPanel()
    {
        if (rewardPanel == null)
            return;
        
        rewardPanel.SetActive(false);
        SetMenuInteractable();
    }
    
    public void SetMenuInteractable()
    {
        bool isMenuInteractable = !settingsPanel.activeSelf 
                                  && !saveLoadPanel.activeSelf 
                                  && !questsMenuPanel.activeSelf
                                  && !rewardPanel.activeSelf
                                  && !weaponsPanel.activeSelf;
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
            menuObserver.ActiveMenu.LockCharacter(false);
            //if (lockedCharacterOverlay != null) lockedCharacterOverlay.SetActive(false); 

            startBuyButton.onClick.AddListener(() => { 
                menuObserver.ActiveMenu.StartGame(); 
            });
        }
        else
        {
            if (startBuyButtonText != null) startBuyButtonText.text = current.characterPrice.ToString();
            if (coinIcon != null) coinIcon.gameObject.SetActive(true); 
            //if (lockedCharacterOverlay != null) lockedCharacterOverlay.SetActive(true); 
            menuObserver.ActiveMenu.LockCharacter(true);
            
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
    
    public void OnClaimClicked()
    {
        DailyRewardManager.Instance.ClaimReward();
        CloseRewardsPanel();
    }

    public void OpenWeaponsPanel()
    {
        if (weaponsPanel == null)
            return;

        weaponsPanel.SetActive(true);
        SetMenuInteractable();
    }
    
    public void CloseWeaponsPanel()
    {
        if (weaponsPanel == null)
            return;
        
        weaponsPanel.SetActive(false);
        SetMenuInteractable();
    }
}
