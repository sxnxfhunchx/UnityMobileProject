using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MetaUIController : MonoBehaviour
{
    [Header("Gold UI")]
    [SerializeField] private TMP_Text goldText;

    private bool isSubscribed = false;

    private void Start()
    {
        InitializeUI();
    }

    private void OnEnable()
    {
        if (!isSubscribed)
        {
            TrySubscribe();
        }
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnProgressUpdated += UpdateMetaUI;
            isSubscribed = true;
        }
    }

    private void Unsubscribe()
    {
        if (QuestManager.Instance != null && isSubscribed)
        {
            QuestManager.Instance.OnProgressUpdated -= UpdateMetaUI;
            isSubscribed = false;
        }
    }

    private void InitializeUI()
    {
        TrySubscribe();
        UpdateMetaUI();
    }

    private void UpdateMetaUI()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("[MetaUIController] Cannot update UI: QuestManager.Instance is null!");
            return;
        }

        if (goldText != null)
        {
            goldText.text = QuestManager.Instance.GetTotalGold().ToString();
        }

        List<QuestChainProgress> activeProgress = QuestManager.Instance.GetCurrentProgress();
        
        if (activeProgress == null || activeProgress.Count == 0)
        {
            Debug.LogWarning("[MetaUIController] Active progress list is empty. Check if scriptable objects are linked to QuestManager!");
            return;
        }

        foreach (var progress in activeProgress)
        {
            if (progress == null) continue;
            
            var config = QuestManager.Instance.GetChainConfig(progress.chainID);
            if (config == null) continue;

            var currentStep = config.GetStep(progress.currentStepIndex);
        }
    }
}