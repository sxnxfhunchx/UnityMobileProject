using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest Configurations")]
    [SerializeField] private List<QuestChainConfig> questChains;

    private MetaProgressSaveData metaProgressData = new MetaProgressSaveData();
    private Dictionary<string, QuestChainProgress> progressMap = new Dictionary<string, QuestChainProgress>();
    private string saveFilePath;

    public event Action OnProgressUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            saveFilePath = Path.Combine(Application.persistentDataPath, "meta_progress.json");
            LoadMetaProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeMissingChains();
    }

    private void InitializeMissingChains()
    {
        if (questChains == null) return;

        bool progressUpdated = false;

        foreach (var config in questChains)
        {
            if (config == null || string.IsNullOrEmpty(config.chainID)) continue;

            if (!progressMap.ContainsKey(config.chainID))
            {
                QuestChainProgress newProgress = new QuestChainProgress
                {
                    chainID = config.chainID,
                    currentStepIndex = 0,
                    currentProgressValue = 0,
                    isChainCompleted = false,
                    isRewardAvailable = false
                };
                
                progressMap.Add(config.chainID, newProgress);
                metaProgressData.chainProgresses.Add(newProgress);
                progressUpdated = true;
            }
        }

        if (progressUpdated)
        {
            SaveMetaProgress();
        }
    }

    public void TrackProgress(QuestType type, int amount)
    {
        if (questChains == null) return;

        bool hasChanges = false;

        foreach (var config in questChains)
        {
            if (config == null || config.questType != type) continue;
            if (!progressMap.TryGetValue(config.chainID, out QuestChainProgress progress)) continue;
            if (progress.isChainCompleted || progress.isRewardAvailable) continue;

            QuestStep currentStep = config.GetStep(progress.currentStepIndex);
            
            if (type == QuestType.ReachLevel)
            {
               
                if (amount > progress.currentProgressValue) 
                {
                    progress.currentProgressValue = amount;
                    hasChanges = true;
                }
                
                if (progress.currentProgressValue >= currentStep.targetValue)
                {
                    progress.currentProgressValue = currentStep.targetValue;
                    progress.isRewardAvailable = true;
                }
            }
            else
            {
                progress.currentProgressValue += amount;
                hasChanges = true;

                if (progress.currentProgressValue >= currentStep.targetValue)
                {
                    progress.currentProgressValue = currentStep.targetValue;
                    progress.isRewardAvailable = true;
                }
            }
        }

        if (hasChanges)
        {
            SaveMetaProgress();
            OnProgressUpdated?.Invoke();
        }
    }

    public void ClaimReward(string chainID)
    {
        if (!progressMap.TryGetValue(chainID, out QuestChainProgress progress)) return;
        if (!progress.isRewardAvailable || progress.isChainCompleted) return;

        QuestChainConfig config = GetChainConfig(chainID);
        if (config == null) return;

        QuestStep completedStep = config.GetStep(progress.currentStepIndex);
        
        metaProgressData.totalGold += completedStep.goldReward;
        Debug.Log($"[QuestManager] Claimed {completedStep.goldReward} Gold from chain {chainID}!");

        progress.isRewardAvailable = false;
        progress.currentStepIndex++;
        progress.currentProgressValue = 0; 

        QuestStep nextStep = config.GetStep(progress.currentStepIndex);
        if (nextStep.stepDescription == "Completed")
        {
            progress.isChainCompleted = true;
            Debug.Log($"[QuestManager] Chain {chainID} has been fully completed!");
        }

        SaveMetaProgress();
        OnProgressUpdated?.Invoke();
    }

    public int GetTotalGold()
    {
        return metaProgressData.totalGold;
    }

    public List<QuestChainProgress> GetCurrentProgress()
    {
        return new List<QuestChainProgress>(metaProgressData.chainProgresses);
    }

    public QuestChainConfig GetChainConfig(string chainID)
    {
        return questChains.Find(c => c != null && c.chainID == chainID);
    }

    public void SaveMetaProgress()
    {
        try
        {
            string jsonString = JsonUtility.ToJson(metaProgressData, true);
            File.WriteAllText(saveFilePath, jsonString, Encoding.UTF8);
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] Failed to save meta progress: {e.Message}");
        }
    }

    private void LoadMetaProgress()
    {
        if (!File.Exists(saveFilePath)) 
        {
            metaProgressData = new MetaProgressSaveData();
            metaProgressData.totalGold = 0;
            metaProgressData.unlockedCharacterIds = new List<string> { "barbarian" };
            return; 
        }
        try
        {
            string jsonString = File.ReadAllText(saveFilePath, Encoding.UTF8);
            metaProgressData = JsonUtility.FromJson<MetaProgressSaveData>(jsonString);

            progressMap.Clear();
            if (metaProgressData.chainProgresses != null)
            {
                foreach (var progress in metaProgressData.chainProgresses)
                {
                    if (progress != null && !string.IsNullOrEmpty(progress.chainID))
                    {
                        progressMap.Add(progress.chainID, progress);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[QuestManager] Failed to load meta progress: {e.Message}");
        }
    }
    
    public void ResetAllQuestsProgress()
    {
        if (questChains == null) return;

        progressMap.Clear();
        metaProgressData.chainProgresses.Clear();
        metaProgressData.totalGold = 0; 

        metaProgressData.unlockedCharacterIds.Clear();
        metaProgressData.unlockedCharacterIds.Add("0"); 

        foreach (var config in questChains)
        {
            if (config == null || string.IsNullOrEmpty(config.chainID)) continue;

            QuestChainProgress newProgress = new QuestChainProgress
            {
                chainID = config.chainID,
                currentStepIndex = 0,
                currentProgressValue = 0,
                isChainCompleted = false,
                isRewardAvailable = false
            };
        
            progressMap.Add(config.chainID, newProgress);
            metaProgressData.chainProgresses.Add(newProgress);
        }

        SaveMetaProgress();
        OnProgressUpdated?.Invoke();
    }
    
    public void SpendGold(int amount)
    {
        metaProgressData.totalGold -= amount;
        SaveMetaProgress();
        OnProgressUpdated?.Invoke();
    }

    public void UnlockCharacter(string characterId)
    {
        if (!metaProgressData.unlockedCharacterIds.Contains(characterId))
        {
            metaProgressData.unlockedCharacterIds.Add(characterId);
            SaveMetaProgress();
        }
    }

    public bool IsUnlocked(string characterId)
    {
        return metaProgressData.unlockedCharacterIds.Contains(characterId);
    }

    public void GainGold(int rewardAmount)
    {
        if (rewardAmount <= 0)
            return;
        
        metaProgressData.totalGold += rewardAmount;
        OnProgressUpdated?.Invoke();
    }
}