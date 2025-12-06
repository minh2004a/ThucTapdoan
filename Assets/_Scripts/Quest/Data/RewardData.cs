using UnityEngine;
using System.Collections.Generic;

namespace QuestSystem
{
    [System.Serializable]
    public class ItemReward
    {
        public ItemSO item;
        public int amount = 1;
        [Range(0f, 100f)]
        public float dropChance = 100f;

        public bool ShouldGiveReward()
        {
            return Random.Range(0f, 100f) <= dropChance;
        }
    }

    [System.Serializable]
    public class NPCReputationReward
    {
        public string npcId;
        public int amount;
    }

    [System.Serializable]
    public class UnlockReward
    {
        public UnlockType unlockType;
        public string targetId;
    }

    [System.Serializable]
    public class QuestReward
    {
        [Header("Immediate Rewards")]
        public int goldReward;
        public List<ItemReward> itemRewards = new List<ItemReward>();
        public int experienceReward;

        [Header("Progression Rewards")]
        public List<NPCReputationReward> reputationRewards = new List<NPCReputationReward>();
        public List<UnlockReward> unlocks = new List<UnlockReward>();

        [Header("Special")]
        public List<string> unlockQuestIds = new List<string>();
    }

    public interface IRewardHandler
    {
        RewardType GetRewardType();
        bool CanHandle(object rewardData);
        bool ProcessReward(object rewardData);
    }

    public class RewardResult
    {
        public bool success;
        public string errorMessage;
        public List<string> processedRewards = new List<string>();
    }
}
