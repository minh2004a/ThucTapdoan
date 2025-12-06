using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace QuestSystem.UI
{
    public class QuestTrackerUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject trackerPanel;
        [SerializeField] private Transform trackedQuestContent;
        [SerializeField] private GameObject trackedQuestPrefab;
        [SerializeField] private int maxTrackedQuests = 3;

        private QuestManager questManager;
        private List<GameObject> trackedQuestItems = new List<GameObject>();

        private void Start()
        {
            questManager = QuestManager.Instance;

            if (questManager != null)
            {
                questManager.OnQuestStarted += OnQuestChanged;
                questManager.OnQuestCompleted += OnQuestChanged;
            }

            UpdateTracker();
        }

        private void OnDestroy()
        {
            if (questManager != null)
            {
                questManager.OnQuestStarted -= OnQuestChanged;
                questManager.OnQuestCompleted -= OnQuestChanged;
            }
        }

        private void Update()
        {
            UpdateTracker();
        }

        private void UpdateTracker()
        {
            if (questManager == null || trackedQuestContent == null) return;

            ClearTracker();

            var activeQuests = questManager.GetAllActiveQuests()
                .Where(q => q.questData.trackInUI)
                .Take(maxTrackedQuests)
                .ToList();

            foreach (var quest in activeQuests)
            {
                CreateTrackedQuestItem(quest);
            }
        }

        private void CreateTrackedQuestItem(QuestInstance quest)
        {
            if (trackedQuestPrefab == null) return;

            var item = Instantiate(trackedQuestPrefab, trackedQuestContent);
            trackedQuestItems.Add(item);

            var texts = item.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = quest.questData.questName;

                var progress = quest.GetProgressText();
                var percent = Mathf.RoundToInt(quest.GetOverallProgress() * 100);
                texts[1].text = $"{progress} ({percent}%)";
            }

            var slider = item.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                slider.value = quest.GetOverallProgress();
            }
        }

        private void ClearTracker()
        {
            foreach (var item in trackedQuestItems)
            {
                Destroy(item);
            }
            trackedQuestItems.Clear();
        }

        private void OnQuestChanged(QuestInstance quest)
        {
            UpdateTracker();
        }

        public void ToggleTracker()
        {
            if (trackerPanel != null)
            {
                trackerPanel.SetActive(!trackerPanel.activeSelf);
            }
        }
    }
}
