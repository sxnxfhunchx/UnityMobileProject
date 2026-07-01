using System;
using System.Collections.Generic;

[Serializable]
public class QuestChainProgress
{
    public string chainID;
    public int currentStepIndex;
    public int currentProgressValue;
    public bool isChainCompleted;
    public bool isRewardAvailable;
}

[Serializable]
public class MetaProgressSaveData
{
    public int totalGold;
    public List<QuestChainProgress> chainProgresses = new List<QuestChainProgress>();
}