using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace QuestSystem.UI
{
    public class DailyQuestUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject dailyPanel;
        [SerializeField] private TMP_Text timeUntilResetText;
        [SerializeField] private TMP_Text streakText;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Button closeButton;

        private DailyQuestManager dailyManager;
        private List<GameObject> slotItems = new List<GameObject>();

        private void Start()
        {
            dailyManager = DailyQuestManager.Instance;

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }

            if (dailyPanel != null)
            {
                dailyPanel.SetActive(false);
            }

            if (dailyManager != null)
            {
                dailyManager.OnDailyQuestsRefreshed += RefreshUI;
                dailyManager.OnStreakChanged += UpdateStreak;
            }
        }

        private void OnDestroy()
        {
            if (dailyManager != null)
            {
                dailyManager.OnDailyQuestsRefreshed -= RefreshUI;
                dailyManager.OnStreakChanged -= UpdateStreak;
            }
        }

        private void Update()
        {
            UpdateTimeUntilReset();
        }

        public void ShowDailyPanel()
        {
            if (dailyPanel != null)
            {
                dailyPanel.SetActive(true);
            }

            RefreshUI();
        }

        public void ClosePanel()
        {
            if (dailyPanel != null)
            {
                dailyPanel.SetActive(false);
            }
        }

        private void RefreshUI()
        {
            if (dailyManager == null) return;

            ClearSlots();

            var slots = dailyManager.GetAllSlots();
            foreach (var slot in slots)
            {
                CreateSlotItem(slot);
            }

            UpdateStreak(dailyManager.ConsecutiveDays);
        }

        private void CreateSlotItem(DailyQuestSlot slot)
        {
            if (slotPrefab == null || slotContainer == null) return;

            var item = Instantiate(slotPrefab, slotContainer);
            slotItems.Add(item);

            var texts = item.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 3 && slot.quest != null)
            {
                texts[0].text = slot.quest.questName;
                texts[1].text = slot.quest.description;
                texts[2].text = $"Reward: {slot.quest.rewards.goldReward}g";
            }

            var buttons = item.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                var acceptButton = buttons[0];
                var rerollButton = buttons[1];

                acceptButton.interactable = slot.state == SlotState.Available;
                acceptButton.onClick.AddListener(() => AcceptQuest(slot.slotIndex));

                bool canReroll = slot.state == SlotState.Available && dailyManager.RemainingRerolls(slot.slotIndex) > 0;
                rerollButton.interactable = canReroll;
                rerollButton.onClick.AddListener(() => RerollQuest(slot.slotIndex));

                var rerollText = rerollButton.GetComponentInChildren<TMP_Text>();
                if (rerollText != null)
                {
                    int remaining = dailyManager.RemainingRerolls(slot.slotIndex);
                    rerollText.text = $"Reroll ({remaining})";
                }
            }

            if (slot.state == SlotState.Completed)
            {
                var completeText = item.GetComponentInChildren<TMP_Text>();
                if (completeText != null)
                {
                    completeText.text = "✓ COMPLETED";
                    completeText.color = Color.green;
                }
            }
        }

        private void AcceptQuest(int slotIndex)
        {
            if (dailyManager != null)
            {
                dailyManager.StartDailyQuest(slotIndex);
                RefreshUI();
            }
        }

        private void RerollQuest(int slotIndex)
        {
            if (dailyManager != null)
            {
                if (dailyManager.RerollSlot(slotIndex))
                {
                    RefreshUI();
                }
            }
        }

        private void UpdateTimeUntilReset()
        {
            if (timeUntilResetText == null || dailyManager == null) return;

            var timeRemaining = dailyManager.GetTimeUntilReset();
            timeUntilResetText.text = $"Reset in: {timeRemaining.Hours}h {timeRemaining.Minutes}m";
        }

        private void UpdateStreak(int days)
        {
            if (streakText != null)
            {
                streakText.text = days > 0 ? $"Streak: {days} days 🔥" : "Streak: 0";
            }
        }

        private void ClearSlots()
        {
            foreach (var item in slotItems)
            {
                Destroy(item);
            }
            slotItems.Clear();
        }
    }
}
