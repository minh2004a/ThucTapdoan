using System;
using System.Collections.Generic;

namespace QuestSystem.Events
{
    public class QuestStartedEvent
    {
        public string questId;
        public QuestData questData;
        public DateTime timestamp;
    }

    public class QuestProgressUpdatedEvent
    {
        public string questId;
        public int objectiveIndex;
        public int oldProgress;
        public int newProgress;
        public bool isComplete;
    }

    public class QuestCompletedEvent
    {
        public string questId;
        public QuestData questData;
        public DateTime completionTime;
    }

    public class QuestFailedEvent
    {
        public string questId;
        public string failReason;
        public bool canRetry;
    }

    public class QuestUnlockedEvent
    {
        public string questId;
        public string unlockedBy;
    }

    public class DailyQuestsRefreshedEvent
    {
        public List<QuestData> newQuests;
        public DateTime resetTime;
    }

    public class MonsterKilledEvent
    {
        public string monsterId;
        public string monsterType;
        public WeaponType weaponUsed;
        public UnityEngine.Vector3 location;
    }

    public class ItemCollectedEvent
    {
        public ItemSO item;
        public int amount;
        public int newCount;
    }

    public class ItemBoughtEvent
    {
        public ItemSO item;
        public int amount;
        public int price;
        public string vendorId;
    }

    public class ItemSoldEvent
    {
        public ItemSO item;
        public int amount;
        public int earnings;
        public string vendorId;
    }

    public class CropPlantedEvent
    {
        public SeedSO seed;
        public UnityEngine.Vector2Int location;
    }

    public class CropHarvestedEvent
    {
        public ItemSO crop;
        public int amount;
        public UnityEngine.Vector2Int location;
    }

    public class NPCInteractionEvent
    {
        public string npcId;
        public string interactionType;
        public DateTime timestamp;
    }

    public class MoneyEarnedEvent
    {
        public int amount;
        public string source;
        public int newTotal;
    }

    public class LocationDiscoveredEvent
    {
        public string locationId;
        public string locationName;
        public DateTime timestamp;
    }
}
