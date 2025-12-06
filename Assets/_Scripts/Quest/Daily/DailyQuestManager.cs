using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using QuestSystem.Events;

namespace QuestSystem
{
    [Serializable]
    public class DailyQuestSlot
    {
        public int slotIndex;
        public QuestData quest;
        public SlotState state;
        public int rerollsUsed;

        public DailyQuestSlot(int index)
        {
            slotIndex = index;
            state = SlotState.Empty;
            rerollsUsed = 0;
        }
    }

    public class DailyQuestManager : MonoBehaviour
    {
        public static DailyQuestManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxDailyQuests = 3;
        [SerializeField] private int resetHour = 6;
        [SerializeField] private int maxRerollsPerSlot = 2;
        [SerializeField] private int rerollCost = 50;

        [Header("Quest Pool")]
        [SerializeField] private List<QuestData> dailyQuestPool = new List<QuestData>();

        [Header("Streak System")]
        [SerializeField] private bool enableStreakBonus = true;
        [SerializeField] private float streakGoldMultiplier = 0.1f;

        private List<DailyQuestSlot> currentSlots;
        private DateTime lastResetTime;
        private HashSet<string> completedToday;
        private int consecutiveDays;
        private QuestManager questManager;

        public event Action OnDailyQuestsRefreshed;
        public event Action<int> OnStreakChanged;

        public int ConsecutiveDays => consecutiveDays;
        public int RemainingRerolls(int slotIndex) => maxRerollsPerSlot - GetSlot(slotIndex)?.rerollsUsed ?? 0;

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
            questManager = QuestManager.Instance;
            currentSlots = new List<DailyQuestSlot>();
            completedToday = new HashSet<string>();
            lastResetTime = DateTime.Now;
            consecutiveDays = 0;

            for (int i = 0; i < maxDailyQuests; i++)
            {
                currentSlots.Add(new DailyQuestSlot(i));
            }

            if (questManager != null)
            {
                questManager.OnQuestCompleted += OnQuestCompleted;
            }
        }

        private void OnDestroy()
        {
            if (questManager != null)
            {
                questManager.OnQuestCompleted -= OnQuestCompleted;
            }
        }

        private void Start()
        {
            GenerateDailyQuests();
        }

        private void Update()
        {
            CheckForReset();
        }

        private void CheckForReset()
        {
            var now = DateTime.Now;

            if (now.Date > lastResetTime.Date)
            {
                if (now.Hour >= resetHour)
                {
                    bool completedYesterday = completedToday.Count > 0;

                    if (completedYesterday)
                    {
                        consecutiveDays++;
                    }
                    else
                    {
                        consecutiveDays = 0;
                    }

                    ResetDailyQuests();
                }
            }
        }

        public void GenerateDailyQuests()
        {
            if (dailyQuestPool.Count == 0)
            {
                Debug.LogWarning("Daily quest pool is empty");
                return;
            }

            var selectedQuests = SelectRandomQuests(maxDailyQuests);

            for (int i = 0; i < currentSlots.Count && i < selectedQuests.Count; i++)
            {
                currentSlots[i].quest = selectedQuests[i];
                currentSlots[i].state = SlotState.Available;
                currentSlots[i].rerollsUsed = 0;
            }

            OnDailyQuestsRefreshed?.Invoke();

            EventDispatcher.Instance.Dispatch(new DailyQuestsRefreshedEvent
            {
                newQuests = selectedQuests,
                resetTime = DateTime.Now
            });
        }

        private List<QuestData> SelectRandomQuests(int count)
        {
            var shuffled = dailyQuestPool.OrderBy(x => UnityEngine.Random.value).ToList();
            return shuffled.Take(count).ToList();
        }

        public bool StartDailyQuest(int slotIndex)
        {
            var slot = GetSlot(slotIndex);
            if (slot == null || slot.quest == null || slot.state != SlotState.Available)
            {
                return false;
            }

            if (questManager.StartQuest(slot.quest.questId))
            {
                slot.state = SlotState.Active;
                return true;
            }

            return false;
        }

        public bool RerollSlot(int slotIndex)
        {
            var slot = GetSlot(slotIndex);
            if (slot == null || slot.state != SlotState.Available)
            {
                return false;
            }

            if (slot.rerollsUsed >= maxRerollsPerSlot)
            {
                Debug.Log("Max rerolls reached for this slot");
                return false;
            }

            var economy = FindObjectOfType<EconomyManager>();
            if (economy != null)
            {
                var wallet = FindObjectOfType<PlayerWallet>();
                if (wallet != null && !wallet.CanAfford(rerollCost))
                {
                    Debug.Log("Not enough gold to reroll");
                    return false;
                }

                if (!wallet.TrySpend(rerollCost))
                {
                    return false;
                }
            }

            var newQuest = GetRandomQuestExcluding(GetCurrentQuests());
            if (newQuest != null)
            {
                slot.quest = newQuest;
                slot.rerollsUsed++;
                return true;
            }

            return false;
        }

        private QuestData GetRandomQuestExcluding(List<QuestData> exclude)
        {
            var available = dailyQuestPool.Where(q => !exclude.Contains(q)).ToList();
            if (available.Count == 0) return null;

            int randomIndex = UnityEngine.Random.Range(0, available.Count);
            return available[randomIndex];
        }

        private List<QuestData> GetCurrentQuests()
        {
            return currentSlots.Select(s => s.quest).Where(q => q != null).ToList();
        }

        private void ResetDailyQuests()
        {
            completedToday.Clear();
            lastResetTime = DateTime.Now;

            foreach (var slot in currentSlots)
            {
                slot.quest = null;
                slot.state = SlotState.Empty;
                slot.rerollsUsed = 0;
            }

            GenerateDailyQuests();

            OnStreakChanged?.Invoke(consecutiveDays);
        }

        private void OnQuestCompleted(QuestInstance quest)
        {
            for (int i = 0; i < currentSlots.Count; i++)
            {
                var slot = currentSlots[i];
                if (slot.quest != null && slot.quest.questId == quest.questId)
                {
                    slot.state = SlotState.Completed;
                    completedToday.Add(quest.questId);

                    if (enableStreakBonus)
                    {
                        ApplyStreakBonus(quest);
                    }

                    break;
                }
            }
        }

        private void ApplyStreakBonus(QuestInstance quest)
        {
            if (consecutiveDays <= 0) return;

            int bonusGold = Mathf.RoundToInt(quest.questData.rewards.goldReward * streakGoldMultiplier * consecutiveDays);
            if (bonusGold > 0)
            {
                var economy = FindObjectOfType<EconomyManager>();
                economy?.AddMoney(bonusGold);
                Debug.Log($"Streak bonus: +{bonusGold}g (Day {consecutiveDays})");
            }
        }

        public DailyQuestSlot GetSlot(int index)
        {
            if (index < 0 || index >= currentSlots.Count) return null;
            return currentSlots[index];
        }

        public List<DailyQuestSlot> GetAllSlots()
        {
            return new List<DailyQuestSlot>(currentSlots);
        }

        public TimeSpan GetTimeUntilReset()
        {
            var now = DateTime.Now;
            var nextReset = now.Date.AddDays(1).AddHours(resetHour);

            if (now.Hour < resetHour)
            {
                nextReset = now.Date.AddHours(resetHour);
            }

            return nextReset - now;
        }

        public int GetCompletedTodayCount()
        {
            return completedToday.Count;
        }
    }
}
