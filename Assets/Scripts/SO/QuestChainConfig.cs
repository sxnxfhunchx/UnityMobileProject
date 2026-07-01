using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct QuestStep
{
    public string stepDescription;
    public int targetValue;
    public int goldReward;
}

[CreateAssetMenu(fileName = "NewQuestChain", menuName = "ScriptableObjects/QuestChainConfig")]
public class QuestChainConfig : ScriptableObject
{
    public string chainID;
    public QuestType questType;
    
    [Header("Static Steps")]
    public List<QuestStep> staticSteps;

    [Header("Infinite Generation Settings")]
    public bool isInfinite;
    public int infiniteStartValue = 150;
    public int infiniteStepIncrement = 50;
    public int infiniteGoldRewardBase = 120;
    
    public QuestStep GetStep(int index)
    {
        if (index < staticSteps.Count)
        {
            return staticSteps[index];
        }

        if (isInfinite)
        {
            int infiniteIndex = index - staticSteps.Count;
            int target = infiniteStartValue + (infiniteIndex * infiniteStepIncrement);
            int reward = infiniteGoldRewardBase + (infiniteIndex * 20);
            
            return new QuestStep
            {
                stepDescription = $"Kill {target} Skeletons",
                targetValue = target,
                goldReward = reward
            };
        }

        return new QuestStep 
        { 
            stepDescription = "Completed", 
            targetValue = int.MaxValue, 
            goldReward = 0 
        };
    }
}