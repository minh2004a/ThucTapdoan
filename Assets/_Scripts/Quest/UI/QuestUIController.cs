using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace QuestSystem.UI
{
    public class QuestUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject questPanel;
        [SerializeField] private Transform questListContent;
        [SerializeField] private GameObject questItemPrefab;

        [Header("Quest Detail")]
        [SerializeField] private TMP_Text questTitleText;
        [SerializeField] private TMP_Text questDescriptionText;
        [SerializeField] private Transform objectiveListContent;
        [SerializeField] private GameObject objectiveItemPrefab;

        [Header("Buttons")]
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button turnInButton;
        [SerializeField] private Button closeButton;

        private QuestManager questManager;
        private QuestData selectedQuest;
        private List<GameObject> questItems = new List<GameObject>();
        private List<GameObject> objectiveItems = new List<GameObject>();

        private void Start()
        {
            questManager = QuestManager.Instance;

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }

            if (acceptButton != null)
            {
                acceptButton.onClick.AddListener(AcceptSelectedQuest);
            }

            if (turnInButton != null)
            {
                turnInButton.onClick.AddListener(TurnInSelectedQuest);
            }

            if (questPanel != null)
            {
                questPanel.SetActive(false);
            }

            if (questManager != null)
            {
                questManager.OnQuestStarted += OnQuestStarted;
                questManager.OnQuestCompleted += OnQuestCompleted;
            }
        }

        private void OnDestroy()
        {
            if (questManager != null)
            {
                questManager.OnQuestStarted -= OnQuestStarted;
                questManager.OnQuestCompleted -= OnQuestCompleted;
            }
        }

        public void ShowQuestPanel()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(true);
            }

            RefreshQuestList();
        }

        public void ClosePanel()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(false);
            }
        }

        public void ShowQuestsFromNPC(NPCQuestGiver npc)
        {
            ShowQuestPanel();

            ClearQuestList();

            var quests = npc.GetAvailableQuests();
            foreach (var quest in quests)
            {
                CreateQuestListItem(quest);
            }

            var activeQuests = questManager.GetAllActiveQuests();
            foreach (var questInst in activeQuests)
            {
                if (questInst.questData.npcGiverId == npc.NPCId)
                {
                    CreateQuestListItem(questInst.questData, questInst);
                }
            }
        }

        private void RefreshQuestList()
        {
            ClearQuestList();

            if (questManager == null) return;

            var activeQuests = questManager.GetAllActiveQuests();
            foreach (var quest in activeQuests)
            {
                CreateQuestListItem(quest.questData, quest);
            }
        }

        private void CreateQuestListItem(QuestData quest, QuestInstance instance = null)
        {
            if (questItemPrefab == null || questListContent == null) return;

            var item = Instantiate(questItemPrefab, questListContent);
            questItems.Add(item);

            var titleText = item.GetComponentInChildren<TMP_Text>();
            if (titleText != null)
            {
                string prefix = instance != null ? "[Active] " : "[Available] ";
                titleText.text = prefix + quest.questName;
            }

            var button = item.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectQuest(quest, instance));
            }
        }

        private void SelectQuest(QuestData quest, QuestInstance instance = null)
        {
            selectedQuest = quest;

            if (questTitleText != null)
            {
                questTitleText.text = quest.questName;
            }

            if (questDescriptionText != null)
            {
                questDescriptionText.text = quest.description;
            }

            ShowObjectives(quest, instance);

            if (acceptButton != null)
            {
                acceptButton.gameObject.SetActive(instance == null);
            }

            if (turnInButton != null)
            {
                bool canTurnIn = instance != null && instance.AreAllObjectivesComplete();
                turnInButton.gameObject.SetActive(canTurnIn);
            }
        }

        private void ShowObjectives(QuestData quest, QuestInstance instance = null)
        {
            ClearObjectiveList();

            if (objectiveItemPrefab == null || objectiveListContent == null) return;

            var objectives = quest.GetAllObjectives();
            for (int i = 0; i < objectives.Count; i++)
            {
                var obj = objectives[i];
                var item = Instantiate(objectiveItemPrefab, objectiveListContent);
                objectiveItems.Add(item);

                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    if (instance != null && i < instance.objectives.Count)
                    {
                        var objInst = instance.objectives[i];
                        string prefix = objInst.isComplete ? "✓ " : "○ ";
                        text.text = prefix + objInst.GetProgressText();
                    }
                    else
                    {
                        text.text = "○ " + obj.description;
                    }
                }
            }
        }

        private void AcceptSelectedQuest()
        {
            if (selectedQuest == null || questManager == null) return;

            if (questManager.StartQuest(selectedQuest.questId))
            {
                RefreshQuestList();
                ClosePanel();
            }
        }

        private void TurnInSelectedQuest()
        {
            if (selectedQuest == null || questManager == null) return;

            var instance = questManager.GetActiveQuest(selectedQuest.questId);
            if (instance != null && instance.AreAllObjectivesComplete())
            {
                questManager.CompleteQuest(selectedQuest.questId);
                RefreshQuestList();
                ClosePanel();
            }
        }

        private void ClearQuestList()
        {
            foreach (var item in questItems)
            {
                Destroy(item);
            }
            questItems.Clear();
        }

        private void ClearObjectiveList()
        {
            foreach (var item in objectiveItems)
            {
                Destroy(item);
            }
            objectiveItems.Clear();
        }

        private void OnQuestStarted(QuestInstance quest)
        {
            RefreshQuestList();
        }

        private void OnQuestCompleted(QuestInstance quest)
        {
            RefreshQuestList();
        }
    }
}
