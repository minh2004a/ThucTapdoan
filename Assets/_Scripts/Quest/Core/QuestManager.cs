using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using QuestSystem.Events;

namespace QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private List<QuestData> allQuestsDatabase = new List<QuestData>();
        [SerializeField] private int maxActiveQuests = 10;

        private Dictionary<string, QuestData> questRegistry;
        private Dictionary<string, QuestInstance> activeQuests;
        private HashSet<string> completedQuests;
        private Dictionary<string, DateTime> questCompletionTimes;
        private Dictionary<string, int> npcReputation;

        private QuestValidator validator;
        private ObjectiveTracker objectiveTracker;
        private RewardProcessor rewardProcessor;

        public event Action<QuestInstance> OnQuestStarted;
        public event Action<QuestInstance> OnQuestCompleted;
        public event Action<QuestInstance> OnQuestFailed;
        public event Action<string, int> OnReputationChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            questRegistry = new Dictionary<string, QuestData>();
            activeQuests = new Dictionary<string, QuestInstance>();
            completedQuests = new HashSet<string>();
            questCompletionTimes = new Dictionary<string, DateTime>();
            npcReputation = new Dictionary<string, int>();

            validator = new QuestValidator(this);

            objectiveTracker = gameObject.AddComponent<ObjectiveTracker>();
            objectiveTracker.Initialize(this);

            rewardProcessor = gameObject.AddComponent<RewardProcessor>();
            rewardProcessor.Initialize();

            RegisterQuests(allQuestsDatabase);
        }

        public void RegisterQuests(List<QuestData> quests)
        {
            if (quests == null) return;

            foreach (var quest in quests)
            {
                RegisterQuest(quest);
            }
        }

        public void RegisterQuest(QuestData quest)
        {
            if (quest == null || string.IsNullOrEmpty(quest.questId)) return;

            if (!questRegistry.ContainsKey(quest.questId))
            {
                questRegistry[quest.questId] = quest;
            }
        }

        public bool StartQuest(string questId)
        {
            if (!questRegistry.TryGetValue(questId, out QuestData questData))
            {
                Debug.LogWarning($"Quest {questId} not found in registry");
                return false;
            }

            if (activeQuests.ContainsKey(questId))
            {
                Debug.LogWarning($"Quest {questId} is already active");
                return false;
            }

            var validation = validator.ValidateAll(questData);
            if (!validation.isValid)
            {
                Debug.Log($"Cannot start quest: {validation.failureReason}");
                return false;
            }

            var questInstance = new QuestInstance(questData);
            questInstance.Start();

            activeQuests[questId] = questInstance;

            foreach (var objective in questInstance.objectives)
            {
                objectiveTracker.RegisterObjective(objective);
            }

            OnQuestStarted?.Invoke(questInstance);

            EventDispatcher.Instance.Dispatch(new QuestStartedEvent
            {
                questId = questId,
                questData = questData,
                timestamp = DateTime.Now
            });

            return true;
        }

        public void CompleteQuest(string questId)
        {
            if (!activeQuests.TryGetValue(questId, out QuestInstance quest)) return;

            if (!quest.AreAllObjectivesComplete())
            {
                Debug.LogWarning($"Quest {questId} objectives not complete");
                return;
            }

            quest.TransitionTo(QuestState.Completed);

            var rewardResult = rewardProcessor.ProcessReward(quest.questData.rewards);

            if (quest.questData.rewards?.reputationRewards != null)
            {
                foreach (var repReward in quest.questData.rewards.reputationRewards)
                {
                    AddReputation(repReward.npcId, repReward.amount);
                }
            }

            if (!quest.questData.isRepeatable)
            {
                completedQuests.Add(questId);
            }

            questCompletionTimes[questId] = DateTime.Now;

            objectiveTracker.UnregisterQuestObjectives(questId);
            activeQuests.Remove(questId);

            OnQuestCompleted?.Invoke(quest);

            EventDispatcher.Instance.Dispatch(new QuestCompletedEvent
            {
                questId = questId,
                questData = quest.questData,
                completionTime = DateTime.Now
            });

            UnlockFollowUpQuests(quest.questData);

            if (quest.questData.autoComplete)
            {
                quest.TransitionTo(QuestState.Archived);
            }
        }

        public void CancelQuest(string questId)
        {
            if (!activeQuests.TryGetValue(questId, out QuestInstance quest)) return;

            objectiveTracker.UnregisterQuestObjectives(questId);
            activeQuests.Remove(questId);
        }

        public void FailQuest(string questId, string reason = "")
        {
            if (!activeQuests.TryGetValue(questId, out QuestInstance quest)) return;

            quest.TransitionTo(QuestState.Failed);

            objectiveTracker.UnregisterQuestObjectives(questId);
            activeQuests.Remove(questId);

            OnQuestFailed?.Invoke(quest);

            EventDispatcher.Instance.Dispatch(new QuestFailedEvent
            {
                questId = questId,
                failReason = reason,
                canRetry = true
            });
        }

        public void OnObjectiveCompleted(ObjectiveInstance objective)
        {
            if (!activeQuests.TryGetValue(objective.questId, out QuestInstance quest)) return;

            EventDispatcher.Instance.Dispatch(new QuestProgressUpdatedEvent
            {
                questId = objective.questId,
                objectiveIndex = objective.objectiveIndex,
                oldProgress = objective.currentProgress - 1,
                newProgress = objective.currentProgress,
                isComplete = objective.isComplete
            });

            if (quest.AreAllObjectivesComplete())
            {
                CompleteQuest(objective.questId);
            }
        }

        private void UnlockFollowUpQuests(QuestData completedQuest)
        {
            if (completedQuest.rewards?.unlockQuestIds == null) return;

            foreach (var questId in completedQuest.rewards.unlockQuestIds)
            {
                EventDispatcher.Instance.Dispatch(new QuestUnlockedEvent
                {
                    questId = questId,
                    unlockedBy = completedQuest.questId
                });
            }
        }

        public List<QuestData> GetAvailableQuestsForNPC(string npcId)
        {
            return questRegistry.Values
                .Where(q => q.npcGiverId == npcId)
                .Where(q => !IsQuestActive(q.questId))
                .Where(q => !IsQuestCompleted(q.questId) || q.isRepeatable)
                .Where(q => validator.CanStartQuest(q))
                .ToList();
        }

        public bool IsQuestActive(string questId)
        {
            return activeQuests.ContainsKey(questId);
        }

        public bool IsQuestCompleted(string questId)
        {
            return completedQuests.Contains(questId);
        }

        public QuestInstance GetActiveQuest(string questId)
        {
            activeQuests.TryGetValue(questId, out QuestInstance quest);
            return quest;
        }

        public int GetActiveQuestCount()
        {
            return activeQuests.Count;
        }

        public List<QuestInstance> GetAllActiveQuests()
        {
            return activeQuests.Values.ToList();
        }

        public DateTime? GetLastCompletionTime(string questId)
        {
            if (questCompletionTimes.TryGetValue(questId, out DateTime time))
            {
                return time;
            }
            return null;
        }

        public void AddReputation(string npcId, int amount)
        {
            if (string.IsNullOrEmpty(npcId)) return;

            if (!npcReputation.ContainsKey(npcId))
            {
                npcReputation[npcId] = 0;
            }

            npcReputation[npcId] += amount;
            OnReputationChanged?.Invoke(npcId, npcReputation[npcId]);
        }

        public int GetReputation(string npcId)
        {
            if (npcReputation.TryGetValue(npcId, out int rep))
            {
                return rep;
            }
            return 0;
        }

        public ReputationLevel GetReputationLevel(string npcId)
        {
            int rep = GetReputation(npcId);

            if (rep >= (int)ReputationLevel.Soulmate) return ReputationLevel.Soulmate;
            if (rep >= (int)ReputationLevel.BestFriend) return ReputationLevel.BestFriend;
            if (rep >= (int)ReputationLevel.Friend) return ReputationLevel.Friend;
            if (rep >= (int)ReputationLevel.Acquaintance) return ReputationLevel.Acquaintance;
            return ReputationLevel.Stranger;
        }

        public QuestData GetQuestData(string questId)
        {
            questRegistry.TryGetValue(questId, out QuestData data);
            return data;
        }

        private void Update()
        {
            CheckTimedQuests();
        }

        private void CheckTimedQuests()
        {
            var expiredQuests = activeQuests.Values
                .Where(q => q.IsExpired())
                .Select(q => q.questId)
                .ToList();

            foreach (var questId in expiredQuests)
            {
                FailQuest(questId, "Time limit exceeded");
            }
        }
    }
}
