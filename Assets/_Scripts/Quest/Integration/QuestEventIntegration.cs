using UnityEngine;
using QuestSystem.Events;

namespace QuestSystem.Integration
{
    public class QuestEventIntegration : MonoBehaviour
    {
        private PlayerInventory inventory;
        private EconomyManager economy;

        private void Start()
        {
            inventory = FindObjectOfType<PlayerInventory>();
            economy = FindObjectOfType<EconomyManager>();

            if (inventory != null)
            {
                HookInventoryEvents();
            }
        }

        private void HookInventoryEvents()
        {
        }

        public void OnMonsterKilled(string monsterType, WeaponType weaponUsed, Vector3 location)
        {
            EventDispatcher.Instance.Dispatch(new MonsterKilledEvent
            {
                monsterId = System.Guid.NewGuid().ToString(),
                monsterType = monsterType,
                weaponUsed = weaponUsed,
                location = location
            });
        }

        public void OnItemCollected(ItemSO item, int amount, int newCount)
        {
            EventDispatcher.Instance.Dispatch(new ItemCollectedEvent
            {
                item = item,
                amount = amount,
                newCount = newCount
            });
        }

        public void OnItemBought(ItemSO item, int amount, int price, string vendorId)
        {
            EventDispatcher.Instance.Dispatch(new ItemBoughtEvent
            {
                item = item,
                amount = amount,
                price = price,
                vendorId = vendorId
            });
        }

        public void OnItemSold(ItemSO item, int amount, int earnings, string vendorId)
        {
            EventDispatcher.Instance.Dispatch(new ItemSoldEvent
            {
                item = item,
                amount = amount,
                earnings = earnings,
                vendorId = vendorId
            });
        }

        public void OnCropPlanted(SeedSO seed, Vector2Int location)
        {
            EventDispatcher.Instance.Dispatch(new CropPlantedEvent
            {
                seed = seed,
                location = location
            });
        }

        public void OnCropHarvested(ItemSO crop, int amount, Vector2Int location)
        {
            EventDispatcher.Instance.Dispatch(new CropHarvestedEvent
            {
                crop = crop,
                amount = amount,
                location = location
            });
        }

        public void OnMoneyEarned(int amount, string source)
        {
            int currentMoney = 0;
            var wallet = FindObjectOfType<PlayerWallet>();
            if (wallet != null)
            {
                currentMoney = wallet.CurrentMoney;
            }

            EventDispatcher.Instance.Dispatch(new MoneyEarnedEvent
            {
                amount = amount,
                source = source,
                newTotal = currentMoney
            });
        }

        public void OnLocationDiscovered(string locationId, string locationName)
        {
            EventDispatcher.Instance.Dispatch(new LocationDiscoveredEvent
            {
                locationId = locationId,
                locationName = locationName,
                timestamp = System.DateTime.Now
            });
        }
    }
}
