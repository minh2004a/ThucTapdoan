using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VendorItem
{
    public ItemSO item;
    [Header("Chi phí tài nguyên đi kèm")]
    [Tooltip("Tài nguyên cần có thêm để mua trang bị này (ngoài tiền).")]
    public ItemSO requiredResource;
    [Tooltip("Số lượng tài nguyên cần có để mua.")]
    public int requiredResourceAmount;
    
    public int GetPrice()
    {
        if (!item) return -1;
        return item.buyPrice;
    }
    public int GetPlayerSellPrice()
    {
        if (!item) return -1;
        return item.sellPrice;
    }

    public bool HasResourceRequirement => requiredResource != null && requiredResourceAmount > 0;
}

public enum VendorType
{
    Equipment,
    Seed
}

public class EquipmentVendor : MonoBehaviour
{
    [Header("Loại cửa hàng")]
    [SerializeField] VendorType vendorType = VendorType.Equipment;

    [Header("Cửa hàng trang bị")]
    [SerializeField] List<VendorItem> stock = new List<VendorItem>();
    [SerializeField] VendorShopUI shopUI;
    [SerializeField] float interactDistance = 3f;
    [SerializeField] Transform player;

    public IReadOnlyList<VendorItem> Stock => stock;

    void Reset()
    {
        shopUI = FindObjectOfType<VendorShopUI>(true);
        var pc = FindObjectOfType<PlayerController>(true);
        if (pc) player = pc.transform;
    }

    void Awake()
    {
        if (!shopUI) shopUI = FindObjectOfType<VendorShopUI>(true);

        if (!player)
        {
            var pc = FindObjectOfType<PlayerController>(true);
            if (pc) player = pc.transform;
        }
    }

    void OnDisable()
    {
        if (shopUI)
        {
            shopUI.Hide(this);
        }
    }

    void OpenShop()
    {
        if (!shopUI) return;
        shopUI.Show(this, stock);
    }

    public void TryOpenShop()
    {
        
        if (!shopUI || UIInputGuard.BlockInputNow()) return;
        if (!IsInRange()) return;

        OpenShop();
    }

    bool IsInRange()
    {
        if (!player) return true;
        return Vector2.Distance(player.position, transform.position) <= interactDistance;
    }

    public bool CanBuyFromPlayer(ItemSO item)
    {
        if (item == null) return false;

        switch (vendorType)
        {
            case VendorType.Equipment:
                return item.category == ItemCategory.Equipment;
            case VendorType.Seed:
                return item.category == ItemCategory.Seed || item.category == ItemCategory.FarmProduct;
            default:
                return false;
        }
    }

    public int GetPlayerSellPrice(ItemSO item)
    {
        if (!item) return -1;
        return item.sellPrice;
    }

    public bool TrySellToVendor(ItemStack stack, PlayerInventory inventory, EconomyManager economy)
    {
        if (stack.item == null || stack.count <= 0) return false;
        if (inventory == null || economy == null) return false;
        if (!CanBuyFromPlayer(stack.item)) return false;

        int unitPrice = GetPlayerSellPrice(stack.item);
        if (unitPrice < 0) return false;

        int payout = unitPrice * stack.count;
        if (!inventory.RemoveItem(stack)) return false;

        economy.AddMoney(payout);
        return true;
    }
}
