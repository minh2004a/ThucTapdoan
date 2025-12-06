using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using QuestSystem.Events;

namespace QuestSystem
{
    public class NPCQuestGiver : MonoBehaviour
    {
        [Header("NPC Identity")]
        [SerializeField] private string npcId;
        [SerializeField] private string npcName;

        [Header("Quest Pool")]
        [SerializeField] private List<QuestData> availableQuests = new List<QuestData>();
        [SerializeField] private bool autoRegisterQuests = true;

        [Header("Interaction")]
        [SerializeField] private float interactionDistance = 2f;
        [SerializeField] private Transform player;

        private QuestManager questManager;
        private List<string> offeredQuestIds;
        private List<string> activeQuestsFromThisNPC;

        public string NPCId => npcId;
        public string NPCName => npcName;

        private void Start()
        {
            questManager = QuestManager.Instance;
            offeredQuestIds = new List<string>();
            activeQuestsFromThisNPC = new List<string>();

            if (autoRegisterQuests)
            {
                RegisterQuests();
            }

            if (player == null)
            {
                var playerObj = FindObjectOfType<PlayerController>();
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }

            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            if (questManager != null)
            {
                questManager.OnQuestStarted += OnQuestStarted;
                questManager.OnQuestCompleted += OnQuestCompleted;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (questManager != null)
            {
                questManager.OnQuestStarted -= OnQuestStarted;
                questManager.OnQuestCompleted -= OnQuestCompleted;
            }
        }

        private void RegisterQuests()
        {
            if (questManager == null) return;

            foreach (var quest in availableQuests)
            {
                if (quest != null)
                {
                    questManager.RegisterQuest(quest);
                }
            }
        }

        public List<QuestData> GetAvailableQuests()
        {
            if (questManager == null) return new List<QuestData>();

            return questManager.GetAvailableQuestsForNPC(npcId);
        }

        public bool HasAvailableQuests()
        {
            return GetAvailableQuests().Count > 0;
        }

        public bool HasActiveQuests()
        {
            UpdateActiveQuests();
            return activeQuestsFromThisNPC.Count > 0;
        }

        public bool HasCompletableQuests()
        {
            if (questManager == null) return false;

            UpdateActiveQuests();

            foreach (var questId in activeQuestsFromThisNPC)
            {
                var quest = questManager.GetActiveQuest(questId);
                if (quest != null && quest.AreAllObjectivesComplete())
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateActiveQuests()
        {
            if (questManager == null) return;

            activeQuestsFromThisNPC = questManager.GetAllActiveQuests()
                .Where(q => q.questData.npcGiverId == npcId)
                .Select(q => q.questId)
                .ToList();
        }

        public void OfferQuest(QuestData quest)
        {
            if (quest == null || questManager == null) return;

            if (!offeredQuestIds.Contains(quest.questId))
            {
                offeredQuestIds.Add(quest.questId);
            }
        }

        public bool AcceptQuest(string questId)
        {
            if (questManager == null) return false;

            bool started = questManager.StartQuest(questId);

            if (started)
            {
                EventDispatcher.Instance.Dispatch(new NPCInteractionEvent
                {
                    npcId = npcId,
                    interactionType = "QuestAccepted",
                    timestamp = System.DateTime.Now
                });
            }

            return started;
        }

        public bool TurnInQuest(string questId)
        {
            if (questManager == null) return false;

            var quest = questManager.GetActiveQuest(questId);
            if (quest == null || quest.questData.npcGiverId != npcId)
            {
                return false;
            }

            if (!quest.AreAllObjectivesComplete())
            {
                Debug.Log("Quest objectives not complete");
                return false;
            }

            questManager.CompleteQuest(questId);

            EventDispatcher.Instance.Dispatch(new NPCInteractionEvent
            {
                npcId = npcId,
                interactionType = "QuestTurnIn",
                timestamp = System.DateTime.Now
            });

            return true;
        }

        public string[] GetDialogue()
        {
            if (HasCompletableQuests())
            {
                return GetCompletionDialogue();
            }
            else if (HasActiveQuests())
            {
                return GetProgressDialogue();
            }
            else if (HasAvailableQuests())
            {
                return GetOfferDialogue();
            }
            else
            {
                return GetGenericDialogue();
            }
        }

        private string[] GetOfferDialogue()
        {
            var quest = GetAvailableQuests().FirstOrDefault();
            if (quest != null && quest.dialogueOnOffer != null && quest.dialogueOnOffer.Length > 0)
            {
                return quest.dialogueOnOffer;
            }
            return new string[] { $"Xin chào! Tôi có công việc cho bạn." };
        }

        private string[] GetProgressDialogue()
        {
            UpdateActiveQuests();
            if (activeQuestsFromThisNPC.Count > 0)
            {
                var questId = activeQuestsFromThisNPC[0];
                var quest = questManager.GetActiveQuest(questId);
                if (quest?.questData.dialogueOnProgress != null && quest.questData.dialogueOnProgress.Length > 0)
                {
                    return quest.questData.dialogueOnProgress;
                }
            }
            return new string[] { "Công việc tiến triển thế nào rồi?" };
        }

        private string[] GetCompletionDialogue()
        {
            UpdateActiveQuests();
            foreach (var questId in activeQuestsFromThisNPC)
            {
                var quest = questManager.GetActiveQuest(questId);
                if (quest != null && quest.AreAllObjectivesComplete())
                {
                    if (quest.questData.dialogueOnComplete != null && quest.questData.dialogueOnComplete.Length > 0)
                    {
                        return quest.questData.dialogueOnComplete;
                    }
                }
            }
            return new string[] { "Bạn đã hoàn thành! Cảm ơn bạn!" };
        }

        private string[] GetGenericDialogue()
        {
            return new string[] { $"Xin chào! Tôi là {npcName}." };
        }

        private void OnQuestStarted(QuestInstance quest)
        {
            if (quest.questData.npcGiverId == npcId)
            {
                Debug.Log($"Quest {quest.questData.questName} started from {npcName}");
            }
        }

        private void OnQuestCompleted(QuestInstance quest)
        {
            if (quest.questData.npcGiverId == npcId)
            {
                Debug.Log($"Quest {quest.questData.questName} completed for {npcName}");
            }
        }

        public bool IsPlayerInRange()
        {
            if (player == null) return false;
            return Vector2.Distance(player.position, transform.position) <= interactionDistance;
        }

        public void OnInteract()
        {
            if (!IsPlayerInRange()) return;

            EventDispatcher.Instance.Dispatch(new NPCInteractionEvent
            {
                npcId = npcId,
                interactionType = "Talk",
                timestamp = System.DateTime.Now
            });
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(npcId))
            {
                npcId = gameObject.name.ToUpper().Replace(" ", "_");
            }

            if (string.IsNullOrEmpty(npcName))
            {
                npcName = gameObject.name;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}
