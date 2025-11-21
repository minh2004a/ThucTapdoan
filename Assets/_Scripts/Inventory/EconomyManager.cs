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
        addResult = default;
        if (!item || amount <= 0 || !inventory || !wallet) return false;

        int unitPrice = pricePerUnit >= 0 ? pricePerUnit : item.buyPrice;
        if (unitPrice < 0) return false;

        int totalCost = unitPrice * amount;
        if (!wallet.CanAfford(totalCost)) return false;

        addResult = inventory.AddItemDetailed(item, amount);
        if (addResult.remaining > 0)
        {
            if (addResult.AddedTotal > 0) inventory.RemoveItem(item, addResult.AddedTotal);
            return false;
        }

        return wallet.TrySpend(totalCost);
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
