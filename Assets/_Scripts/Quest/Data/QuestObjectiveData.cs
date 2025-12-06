using UnityEngine;

namespace QuestSystem
{
    [System.Serializable]
    public abstract class QuestObjectiveData
    {
        public string objectiveId;
        public string description;
        public int targetAmount = 1;
        public bool optional;
        public bool hidden;

        public abstract ObjectiveType GetObjectiveType();
        public abstract bool ValidateCompletion(int currentProgress);
        public abstract string GetProgressText(int currentProgress);
    }

    [System.Serializable]
    public class CollectObjectiveData : QuestObjectiveData
    {
        public ItemSO targetItem;
        public bool consumeOnComplete;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Collect;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            string itemName = targetItem != null ? targetItem.displayName : "item";
            return $"{currentProgress}/{targetAmount} {itemName}";
        }
    }

    [System.Serializable]
    public class KillObjectiveData : QuestObjectiveData
    {
        public string targetMonsterType;
        public WeaponType requiredWeapon = WeaponType.None;
        public string specificArea;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Kill;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            return $"Tiêu diệt {targetMonsterType}: {currentProgress}/{targetAmount}";
        }
    }

    [System.Serializable]
    public class TalkToObjectiveData : QuestObjectiveData
    {
        public string targetNPCId;
        public string specificDialogueId;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.TalkTo;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= 1;
        }

        public override string GetProgressText(int currentProgress)
        {
            return currentProgress >= 1 ? "✓ Đã nói chuyện" : "Nói chuyện với NPC";
        }
    }

    [System.Serializable]
    public class BuyObjectiveData : QuestObjectiveData
    {
        public ItemSO targetItem;
        public string fromSpecificVendor;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Buy;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            string itemName = targetItem != null ? targetItem.displayName : "item";
            return $"Mua {itemName}: {currentProgress}/{targetAmount}";
        }
    }

    [System.Serializable]
    public class SellObjectiveData : QuestObjectiveData
    {
        public ItemSO targetItem;
        public string toSpecificVendor;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Sell;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            string itemName = targetItem != null ? targetItem.displayName : "item";
            return $"Bán {itemName}: {currentProgress}/{targetAmount}";
        }
    }

    [System.Serializable]
    public class PlantObjectiveData : QuestObjectiveData
    {
        public SeedSO targetSeed;
        public bool anySeasonalCrop;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Plant;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            return $"Trồng cây: {currentProgress}/{targetAmount}";
        }
    }

    [System.Serializable]
    public class HarvestObjectiveData : QuestObjectiveData
    {
        public ItemSO targetCrop;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Harvest;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            string cropName = targetCrop != null ? targetCrop.displayName : "crop";
            return $"Thu hoạch {cropName}: {currentProgress}/{targetAmount}";
        }
    }

    [System.Serializable]
    public class ExploreObjectiveData : QuestObjectiveData
    {
        public string targetLocation;
        public bool revealArea;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Explore;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= 1;
        }

        public override string GetProgressText(int currentProgress)
        {
            return currentProgress >= 1 ? $"✓ Đã khám phá {targetLocation}" : $"Khám phá {targetLocation}";
        }
    }

    [System.Serializable]
    public class DeliverObjectiveData : QuestObjectiveData
    {
        public ItemSO itemToDeliver;
        public string targetNPC;
        public bool removeItemOnDeliver = true;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Deliver;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            string itemName = itemToDeliver != null ? itemToDeliver.displayName : "item";
            return $"Giao {itemName} cho {targetNPC}: {currentProgress}/{targetAmount}";
        }
    }

    [System.Serializable]
    public class EarnMoneyObjectiveData : QuestObjectiveData
    {
        public MoneyTrackingMode trackingMode = MoneyTrackingMode.Earned;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.EarnMoney;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            return $"Kiếm {currentProgress}/{targetAmount}g";
        }
    }

    [System.Serializable]
    public class MineObjectiveData : QuestObjectiveData
    {
        public ItemSO targetMineral;

        public override ObjectiveType GetObjectiveType() => ObjectiveType.Mine;

        public override bool ValidateCompletion(int currentProgress)
        {
            return currentProgress >= targetAmount;
        }

        public override string GetProgressText(int currentProgress)
        {
            string mineralName = targetMineral != null ? targetMineral.displayName : "mineral";
            return $"Khai thác {mineralName}: {currentProgress}/{targetAmount}";
        }
    }
}
