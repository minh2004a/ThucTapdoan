using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QuestSystem.Events;

namespace QuestSystem
{
    public class ObjectiveTracker : MonoBehaviour
    {
        private QuestManager questManager;
        private Dictionary<ObjectiveType, List<ObjectiveInstance>> objectivesByType;
        private List<ObjectiveInstance> allTrackedObjectives;

        public void Initialize(QuestManager manager)
        {
            questManager = manager;
            objectivesByType = new Dictionary<ObjectiveType, List<ObjectiveInstance>>();
            allTrackedObjectives = new List<ObjectiveInstance>();

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventDispatcher.Instance.Subscribe<ItemCollectedEvent>(OnItemCollected);
            EventDispatcher.Instance.Subscribe<MonsterKilledEvent>(OnMonsterKilled);
            EventDispatcher.Instance.Subscribe<NPCInteractionEvent>(OnNPCInteraction);
            EventDispatcher.Instance.Subscribe<ItemBoughtEvent>(OnItemBought);
            EventDispatcher.Instance.Subscribe<ItemSoldEvent>(OnItemSold);
            EventDispatcher.Instance.Subscribe<CropPlantedEvent>(OnCropPlanted);
            EventDispatcher.Instance.Subscribe<CropHarvestedEvent>(OnCropHarvested);
            EventDispatcher.Instance.Subscribe<MoneyEarnedEvent>(OnMoneyEarned);
            EventDispatcher.Instance.Subscribe<LocationDiscoveredEvent>(OnLocationDiscovered);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.Unsubscribe<ItemCollectedEvent>(OnItemCollected);
            EventDispatcher.Instance.Unsubscribe<MonsterKilledEvent>(OnMonsterKilled);
            EventDispatcher.Instance.Unsubscribe<NPCInteractionEvent>(OnNPCInteraction);
            EventDispatcher.Instance.Unsubscribe<ItemBoughtEvent>(OnItemBought);
            EventDispatcher.Instance.Unsubscribe<ItemSoldEvent>(OnItemSold);
            EventDispatcher.Instance.Unsubscribe<CropPlantedEvent>(OnCropPlanted);
            EventDispatcher.Instance.Unsubscribe<CropHarvestedEvent>(OnCropHarvested);
            EventDispatcher.Instance.Unsubscribe<MoneyEarnedEvent>(OnMoneyEarned);
            EventDispatcher.Instance.Unsubscribe<LocationDiscoveredEvent>(OnLocationDiscovered);
        }

        public void RegisterObjective(ObjectiveInstance objective)
        {
            if (objective == null) return;

            var type = objective.objectiveData.GetObjectiveType();

            if (!objectivesByType.ContainsKey(type))
            {
                objectivesByType[type] = new List<ObjectiveInstance>();
            }

            objectivesByType[type].Add(objective);
            allTrackedObjectives.Add(objective);
        }

        public void UnregisterObjective(ObjectiveInstance objective)
        {
            if (objective == null) return;

            var type = objective.objectiveData.GetObjectiveType();

            if (objectivesByType.ContainsKey(type))
            {
                objectivesByType[type].Remove(objective);
            }

            allTrackedObjectives.Remove(objective);
        }

        public void UnregisterQuestObjectives(string questId)
        {
            var toRemove = allTrackedObjectives.Where(o => o.questId == questId).ToList();
            foreach (var obj in toRemove)
            {
                UnregisterObjective(obj);
            }
        }

        private void OnItemCollected(ItemCollectedEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.Collect, obj =>
            {
                var collectData = obj.objectiveData as CollectObjectiveData;
                if (collectData != null && collectData.targetItem == e.item)
                {
                    obj.SetProgress(e.newCount);
                    return true;
                }
                return false;
            });

            UpdateObjectivesOfType(ObjectiveType.Mine, obj =>
            {
                var mineData = obj.objectiveData as MineObjectiveData;
                if (mineData != null && mineData.targetMineral == e.item)
                {
                    obj.UpdateProgress(e.amount);
                    return true;
                }
                return false;
            });
        }

        private void OnMonsterKilled(MonsterKilledEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.Kill, obj =>
            {
                var killData = obj.objectiveData as KillObjectiveData;
                if (killData != null && killData.targetMonsterType == e.monsterType)
                {
                    if (killData.requiredWeapon == WeaponType.None || killData.requiredWeapon == e.weaponUsed)
                    {
                        obj.UpdateProgress(1);
                        return true;
                    }
                }
                return false;
            });
        }

        private void OnNPCInteraction(NPCInteractionEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.TalkTo, obj =>
            {
                var talkData = obj.objectiveData as TalkToObjectiveData;
                if (talkData != null && talkData.targetNPCId == e.npcId)
                {
                    obj.SetProgress(1);
                    return true;
                }
                return false;
            });

            UpdateObjectivesOfType(ObjectiveType.Deliver, obj =>
            {
                var deliverData = obj.objectiveData as DeliverObjectiveData;
                if (deliverData != null && deliverData.targetNPC == e.npcId)
                {
                    return false;
                }
                return false;
            });
        }

        private void OnItemBought(ItemBoughtEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.Buy, obj =>
            {
                var buyData = obj.objectiveData as BuyObjectiveData;
                if (buyData != null && buyData.targetItem == e.item)
                {
                    if (string.IsNullOrEmpty(buyData.fromSpecificVendor) || buyData.fromSpecificVendor == e.vendorId)
                    {
                        obj.UpdateProgress(e.amount);
                        return true;
                    }
                }
                return false;
            });
        }

        private void OnItemSold(ItemSoldEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.Sell, obj =>
            {
                var sellData = obj.objectiveData as SellObjectiveData;
                if (sellData != null && sellData.targetItem == e.item)
                {
                    if (string.IsNullOrEmpty(sellData.toSpecificVendor) || sellData.toSpecificVendor == e.vendorId)
                    {
                        obj.UpdateProgress(e.amount);
                        return true;
                    }
                }
                return false;
            });
        }

        private void OnCropPlanted(CropPlantedEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.Plant, obj =>
            {
                var plantData = obj.objectiveData as PlantObjectiveData;
                if (plantData != null)
                {
                    if (plantData.anySeasonalCrop || plantData.targetSeed == e.seed)
                    {
                        obj.UpdateProgress(1);
                        return true;
                    }
                }
                return false;
            });
        }

        private void OnCropHarvested(CropHarvestedEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.Harvest, obj =>
            {
                var harvestData = obj.objectiveData as HarvestObjectiveData;
                if (harvestData != null && harvestData.targetCrop == e.crop)
                {
                    obj.UpdateProgress(e.amount);
                    return true;
                }
                return false;
            });
        }

        private void OnMoneyEarned(MoneyEarnedEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.EarnMoney, obj =>
            {
                var moneyData = obj.objectiveData as EarnMoneyObjectiveData;
                if (moneyData != null)
                {
                    if (moneyData.trackingMode == MoneyTrackingMode.Total)
                    {
                        obj.SetProgress(e.newTotal);
                    }
                    else
                    {
                        obj.UpdateProgress(e.amount);
                    }
                    return true;
                }
                return false;
            });
        }

        private void OnLocationDiscovered(LocationDiscoveredEvent e)
        {
            UpdateObjectivesOfType(ObjectiveType.Explore, obj =>
            {
                var exploreData = obj.objectiveData as ExploreObjectiveData;
                if (exploreData != null && exploreData.targetLocation == e.locationId)
                {
                    obj.SetProgress(1);
                    return true;
                }
                return false;
            });
        }

        private void UpdateObjectivesOfType(ObjectiveType type, System.Func<ObjectiveInstance, bool> updateFunc)
        {
            if (!objectivesByType.TryGetValue(type, out var objectives)) return;

            foreach (var obj in objectives.ToList())
            {
                if (obj.isComplete) continue;

                bool updated = updateFunc(obj);
                if (updated && obj.CheckCompletion())
                {
                    questManager.OnObjectiveCompleted(obj);
                }
            }
        }

        public List<ObjectiveInstance> GetObjectivesForQuest(string questId)
        {
            return allTrackedObjectives.Where(o => o.questId == questId).ToList();
        }

        public float GetProgressForObjective(string questId, int objectiveIndex)
        {
            var obj = allTrackedObjectives.FirstOrDefault(o => o.questId == questId && o.objectiveIndex == objectiveIndex);
            return obj?.GetProgressPercent() ?? 0f;
        }
    }
}
