using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSlotView : MonoBehaviour
{
    [Header("UI Text Elements")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text rewardText;

    [Header("Progress Bar Settings")]
    [SerializeField] private Image fillImage; 

    [Header("Interaction")]
    [SerializeField] private Button claimButton;

    private string currentChainID;

    public void Setup(string chainID, string description, int currentProgress, int targetValue, int reward, bool isRewardAvailable, bool isCompleted)
    {
        currentChainID = chainID;
        
        if (descriptionText != null) descriptionText.text = description;
        if (rewardText != null) rewardText.text = $"+{reward}";

        if (fillImage != null)
        {
            if (targetValue > 0)
            {
                fillImage.fillAmount = (float)currentProgress / targetValue;
            }
            else
            {
                fillImage.fillAmount = 0f;
            }
        }

        if (progressText != null)
        {
            progressText.text = isCompleted ? "COMPLETED" : $"{currentProgress} / {targetValue}";
        }

        if (claimButton != null)
        {
            claimButton.interactable = isRewardAvailable && !isCompleted;
            
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(OnClaimClicked);
        }
    }

    private void OnClaimClicked()
    {
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(currentChainID))
        {
            QuestManager.Instance.ClaimReward(currentChainID);
        }
    }
}