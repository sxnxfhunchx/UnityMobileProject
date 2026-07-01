using System.Collections.Generic;
using UnityEngine;

public class QuestMenuController : MonoBehaviour
{
    [Header("Static UI Slots")]
    [SerializeField] private List<QuestSlotView> staticSlots; 

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnProgressUpdated += RefreshQuestMenu;
        }
        
        RefreshQuestMenu();
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnProgressUpdated -= RefreshQuestMenu;
        }
    }

    public void RefreshQuestMenu()
    {
        if (QuestManager.Instance == null || staticSlots == null) return;

        List<QuestChainProgress> activeProgress = QuestManager.Instance.GetCurrentProgress();

        for (int i = 0; i < activeProgress.Count; i++)
        {
            if (i >= staticSlots.Count || staticSlots[i] == null) break;

            QuestChainProgress progress = activeProgress[i];
            if (progress == null) continue;

            QuestChainConfig config = QuestManager.Instance.GetChainConfig(progress.chainID);
            if (config == null) continue;

            QuestStep currentStep = config.GetStep(progress.currentStepIndex);

            staticSlots[i].Setup(
                progress.chainID,
                currentStep.stepDescription,
                progress.currentProgressValue,
                currentStep.targetValue,
                currentStep.goldReward,
                progress.isRewardAvailable,
                progress.isChainCompleted
            );
        }
    }
}