using UnityEngine;
using System.Collections.Generic;

namespace QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [Header("Identity")]
        public string questId;
        public string questName;
        [TextArea(3, 6)]
        public string description;
        public Sprite icon;

        [Header("Classification")]
        public QuestType questType;
        public QuestCategory category;
        public QuestTier tier = QuestTier.Easy;

        [Header("Requirements")]
        public int minPlayerLevel;
        public List<QuestData> prerequisites = new List<QuestData>();
        public int requiredReputation;
        public string requiredNPCId;

        [Header("Objectives")]
        public List<CollectObjectiveData> collectObjectives = new List<CollectObjectiveData>();
        public List<KillObjectiveData> killObjectives = new List<KillObjectiveData>();
        public List<TalkToObjectiveData> talkObjectives = new List<TalkToObjectiveData>();
        public List<BuyObjectiveData> buyObjectives = new List<BuyObjectiveData>();
        public List<SellObjectiveData> sellObjectives = new List<SellObjectiveData>();
        public List<PlantObjectiveData> plantObjectives = new List<PlantObjectiveData>();
        public List<HarvestObjectiveData> harvestObjectives = new List<HarvestObjectiveData>();
        public List<DeliverObjectiveData> deliverObjectives = new List<DeliverObjectiveData>();
        public List<EarnMoneyObjectiveData> earnMoneyObjectives = new List<EarnMoneyObjectiveData>();
        public ObjectiveOrder objectiveOrder = ObjectiveOrder.Parallel;

        [Header("Rewards")]
        public QuestReward rewards;

        [Header("NPC & Dialogue")]
        public string npcGiverId;
        [TextArea(2, 4)]
        public string[] dialogueOnOffer;
        [TextArea(2, 4)]
        public string[] dialogueOnProgress;
        [TextArea(2, 4)]
        public string[] dialogueOnComplete;

        [Header("Behavior")]
        public bool isRepeatable;
        public float cooldownHours;
        public float timeLimit;
        public bool autoComplete;
        public bool trackInUI = true;

        public int GetTotalObjectiveCount()
        {
            return collectObjectives.Count + killObjectives.Count + talkObjectives.Count +
                   buyObjectives.Count + sellObjectives.Count + plantObjectives.Count +
                   harvestObjectives.Count + deliverObjectives.Count + earnMoneyObjectives.Count;
        }

        public List<QuestObjectiveData> GetAllObjectives()
        {
            var all = new List<QuestObjectiveData>();
            all.AddRange(collectObjectives);
            all.AddRange(killObjectives);
            all.AddRange(talkObjectives);
            all.AddRange(buyObjectives);
            all.AddRange(sellObjectives);
            all.AddRange(plantObjectives);
            all.AddRange(harvestObjectives);
            all.AddRange(deliverObjectives);
            all.AddRange(earnMoneyObjectives);
            return all;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(questId))
            {
                questId = name.ToUpper().Replace(" ", "_");
            }
        }
    }
}
