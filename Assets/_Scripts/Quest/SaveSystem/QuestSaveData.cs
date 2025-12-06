using System;
using System.Collections.Generic;

namespace QuestSystem.SaveSystem
{
    [Serializable]
    public class QuestSaveData
    {
        public List<ActiveQuestSave> activeQuests = new List<ActiveQuestSave>();
        public List<string> completedQuests = new List<string>();
        public DailyQuestSaveData dailyQuestState;
        public Dictionary<string, int> npcReputation = new Dictionary<string, int>();
        public List<string> unlockedQuests = new List<string>();
        public List<QuestHistoryEntry> questHistory = new List<QuestHistoryEntry>();
        public int saveVersion = 1;
    }

    [Serializable]
    public class ActiveQuestSave
    {
        public string questId;
        public string state;
        public long startTimeTicks;
        public List<ObjectiveSave> objectives = new List<ObjectiveSave>();
    }

    [Serializable]
    public class ObjectiveSave
    {
        public int objectiveIndex;
        public int currentProgress;
        public bool isComplete;
        public long? completionTimeTicks;
    }

    [Serializable]
    public class DailyQuestSaveData
    {
        public long lastResetTimeTicks;
        public List<DailySlotSave> currentSlots = new List<DailySlotSave>();
        public List<string> completedToday = new List<string>();
        public int streakDays;
    }

    [Serializable]
    public class DailySlotSave
    {
        public int slotIndex;
        public string questId;
        public string state;
        public int rerollsUsed;
    }

    [Serializable]
    public class QuestHistoryEntry
    {
        public string questId;
        public long completedTimeTicks;
        public bool wasDaily;
    }
}
