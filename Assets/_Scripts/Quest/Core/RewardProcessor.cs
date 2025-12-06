using UnityEngine;
using System.Collections.Generic;

namespace QuestSystem
{
    public class RewardProcessor : MonoBehaviour
    {
        private PlayerInventory inventory;
        private EconomyManager economy;
        private Dictionary<RewardType, IRewardHandler> handlers;

        public void Initialize()
        {
            inventory = FindObjectOfType<PlayerInventory>();
            economy = FindObjectOfType<EconomyManager>();
            handlers = new Dictionary<RewardType, IRewardHandler>();

            RegisterDefaultHandlers();
        }

        private void RegisterDefaultHandlers()
        {
            RegisterHandler(new GoldRewardHandler(economy));
            RegisterHandler(new ItemRewardHandler(inventory));
            RegisterHandler(new ReputationRewardHandler());
        }

        public void RegisterHandler(IRewardHandler handler)
        {
            if (handler == null) return;
            handlers[handler.GetRewardType()] = handler;
        }

        public RewardResult ProcessReward(QuestReward reward)
        {
            var result = new RewardResult { success = true };

            if (reward == null)
            {
                result.success = false;
                result.errorMessage = "Reward data is null";
                return result;
            }

            if (!ValidateReward(reward, out string validationError))
            {
                result.success = false;
                result.errorMessage = validationError;
                return result;
            }

            ProcessGoldReward(reward.goldReward, result);
            ProcessItemRewards(reward.itemRewards, result);
            ProcessReputationRewards(reward.reputationRewards, result);
            ProcessUnlocks(reward.unlocks, result);

            return result;
        }

        private bool ValidateReward(QuestReward reward, out string error)
        {
            error = "";

            if (reward.itemRewards != null && reward.itemRewards.Count > 0)
            {
                if (inventory == null)
                {
                    error = "Inventory system not found";
                    return false;
                }
            }

            if (reward.goldReward > 0)
            {
                if (economy == null)
                {
                    error = "Economy system not found";
                    return false;
                }
            }

            return true;
        }

        private void ProcessGoldReward(int amount, RewardResult result)
        {
            if (amount <= 0) return;

            if (handlers.TryGetValue(RewardType.Gold, out var handler))
            {
                if (handler.ProcessReward(amount))
                {
                    result.processedRewards.Add($"Gold: {amount}");
                }
            }
        }

        private void ProcessItemRewards(List<ItemReward> items, RewardResult result)
        {
            if (items == null || items.Count == 0) return;

            foreach (var itemReward in items)
            {
                if (itemReward.ShouldGiveReward())
                {
                    if (handlers.TryGetValue(RewardType.Item, out var handler))
                    {
                        if (handler.ProcessReward(itemReward))
                        {
                            result.processedRewards.Add($"Item: {itemReward.item.displayName} x{itemReward.amount}");
                        }
                    }
                }
            }
        }

        private void ProcessReputationRewards(List<NPCReputationReward> reputations, RewardResult result)
        {
            if (reputations == null || reputations.Count == 0) return;

            foreach (var repReward in reputations)
            {
                if (handlers.TryGetValue(RewardType.Reputation, out var handler))
                {
                    if (handler.ProcessReward(repReward))
                    {
                        result.processedRewards.Add($"Reputation: {repReward.npcId} +{repReward.amount}");
                    }
                }
            }
        }

        private void ProcessUnlocks(List<UnlockReward> unlocks, RewardResult result)
        {
            if (unlocks == null || unlocks.Count == 0) return;

            foreach (var unlock in unlocks)
            {
                RewardType rewardType = unlock.unlockType switch
                {
                    UnlockType.Quest => RewardType.QuestUnlock,
                    UnlockType.ShopItem => RewardType.ShopUnlock,
                    UnlockType.Area => RewardType.AreaUnlock,
                    UnlockType.Recipe => RewardType.Recipe,
                    _ => RewardType.Custom
                };

                if (handlers.TryGetValue(rewardType, out var handler))
                {
                    if (handler.ProcessReward(unlock))
                    {
                        result.processedRewards.Add($"Unlocked: {unlock.targetId}");
                    }
                }
            }
        }
    }

    public class GoldRewardHandler : IRewardHandler
    {
        private EconomyManager economy;

        public GoldRewardHandler(EconomyManager economyManager)
        {
            economy = economyManager;
        }

        public RewardType GetRewardType() => RewardType.Gold;

        public bool CanHandle(object rewardData)
        {
            return rewardData is int;
        }

        public bool ProcessReward(object rewardData)
        {
            if (economy == null || !(rewardData is int amount)) return false;

            economy.AddMoney(amount);
            return true;
        }
    }

    public class ItemRewardHandler : IRewardHandler
    {
        private PlayerInventory inventory;

        public ItemRewardHandler(PlayerInventory playerInventory)
        {
            inventory = playerInventory;
        }

        public RewardType GetRewardType() => RewardType.Item;

        public bool CanHandle(object rewardData)
        {
            return rewardData is ItemReward;
        }

        public bool ProcessReward(object rewardData)
        {
            if (inventory == null || !(rewardData is ItemReward itemReward)) return false;

            return inventory.AddItem(itemReward.item, itemReward.amount);
        }
    }

    public class ReputationRewardHandler : IRewardHandler
    {
        public RewardType GetRewardType() => RewardType.Reputation;

        public bool CanHandle(object rewardData)
        {
            return rewardData is NPCReputationReward;
        }

        public bool ProcessReward(object rewardData)
        {
            if (!(rewardData is NPCReputationReward repReward)) return false;

            var reputationManager = Object.FindObjectOfType<NPCReputationManager>();
            if (reputationManager != null)
            {
                reputationManager.AddReputation(repReward.npcId, repReward.amount);
                return true;
            }

            return false;
        }
    }
}
