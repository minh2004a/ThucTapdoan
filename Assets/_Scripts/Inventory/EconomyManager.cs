using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerWallet wallet;

    void Awake()
    {
        if (!inventory) inventory = FindObjectOfType<PlayerInventory>(true);
        if (!wallet && inventory) wallet = inventory.GetComponent<PlayerWallet>();
    }

    public bool TryBuy(ItemSO item, int amount, out InventoryAddResult addResult, int pricePerUnit = -1)
    {
        return TryBuyWithMaterials(item, amount, null, out addResult, pricePerUnit);
    }

    public bool TryBuyWithMaterials(
        ItemSO item,
        int amount,
        IReadOnlyList<VendorMaterialCost> materialCosts,
        out InventoryAddResult addResult,
        int pricePerUnit = -1)
    {
        addResult = default;
        if (!item || amount <= 0 || !inventory || !wallet) return false;

        int unitPrice = pricePerUnit >= 0 ? pricePerUnit : item.buyPrice;
        if (unitPrice < 0) return false;

        int totalCost = unitPrice * amount;
        if (!wallet.CanAfford(totalCost)) return false;

        var validCosts = BuildValidCosts(materialCosts);
        if (!HasMaterials(validCosts)) return false;

        addResult = inventory.AddItemDetailed(item, amount);
        if (addResult.remaining > 0)
        {
            if (addResult.AddedTotal > 0) inventory.RemoveItem(item, addResult.AddedTotal);
            return false;
        }

        var consumed = new List<VendorMaterialCost>();
        if (!TryConsumeMaterials(validCosts, consumed))
        {
            if (addResult.AddedTotal > 0) inventory.RemoveItem(item, addResult.AddedTotal);
            return false;
        }

        if (!wallet.TrySpend(totalCost))
        {
            RefundMaterials(consumed);
            if (addResult.AddedTotal > 0) inventory.RemoveItem(item, addResult.AddedTotal);
            return false;
        }

        return true;
    }

    List<VendorMaterialCost> BuildValidCosts(IReadOnlyList<VendorMaterialCost> costs)
    {
        if (costs == null || costs.Count == 0) return new List<VendorMaterialCost>();

        var valid = new List<VendorMaterialCost>(costs.Count);
        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];
            if (cost.IsValid)
            {
                valid.Add(cost);
            }
        }
        return valid;
    }

    bool HasMaterials(List<VendorMaterialCost> costs)
    {
        if (costs == null || costs.Count == 0) return true;
        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];
            if (!inventory.HasItem(cost.item, cost.amount)) return false;
        }
        return true;
    }

    bool TryConsumeMaterials(List<VendorMaterialCost> costs, List<VendorMaterialCost> consumed)
    {
        consumed.Clear();
        if (costs == null || costs.Count == 0) return true;

        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];
            if (inventory.RemoveItem(cost.item, cost.amount))
            {
                consumed.Add(cost);
                continue;
            }

            // rollback
            RefundMaterials(consumed);
            consumed.Clear();
            return false;
        }

        return true;
    }

    void RefundMaterials(List<VendorMaterialCost> consumed)
    {
        if (consumed == null || consumed.Count == 0) return;

        for (int i = 0; i < consumed.Count; i++)
        {
            var cost = consumed[i];
            inventory.AddItem(cost.item, cost.amount);
        }
    }

    public bool TrySell(ItemSO item, int amount, out int payout, int pricePerUnit = -1)
    {
        payout = 0;
        if (!item || amount <= 0 || !inventory || !wallet) return false;

        int unitPrice = pricePerUnit >= 0 ? pricePerUnit : item.sellPrice;
        if (unitPrice < 0) return false;

        if (!inventory.HasItem(item, amount)) return false;
        if (!inventory.RemoveItem(item, amount)) return false;

        payout = unitPrice * amount;
        wallet.AddMoney(payout);
        return true;
    }
}