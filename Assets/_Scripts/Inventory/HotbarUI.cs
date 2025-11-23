
// HotbarUI.cs
using System.Linq;
using UnityEngine;
// Quản lý giao diện thanh công cụ (hotbar) của người chơi
public class HotbarUI : MonoBehaviour
{ 
    [SerializeField] PlayerInventory inv;
    [SerializeField] HotbarSlotUI[] slots;
    [SerializeField] VendorShopUI vendorShopUI;
    [SerializeField] EconomyManager economy;
    PlayerUseConsumable consumableUser;
    [Header("Item Info Panel")]
    [SerializeField] ItemInfoUI infoPanel;   // <--- thêm
    void Awake()
    {
        if (!inv) inv = FindObjectOfType<PlayerInventory>();
        if (!vendorShopUI) vendorShopUI = FindObjectOfType<VendorShopUI>(true);
        if (!economy) economy = FindObjectOfType<EconomyManager>(true);
        if (inv) consumableUser = inv.GetComponent<PlayerUseConsumable>();
        if (slots == null || slots.Length == 0 || slots.Any(s => s == null))
            slots = GetComponentsInChildren<HotbarSlotUI>(true)
                    .OrderBy(s => s.transform.GetSiblingIndex()).ToArray();
    }
    void OnEnable()
    {
        if (!inv) return;
        inv.SelectedChanged += OnChanged;
        inv.HotbarChanged += OnChanged;
        if (inv.selected < 0 || inv.selected >= inv.hotbar.Length) inv.SelectSlot(0);
        Refresh();
    }
    void OnDisable()
    {
        if (!inv) return;
        inv.SelectedChanged -= OnChanged;
        inv.HotbarChanged -= OnChanged;
    }
    void OnChanged(int _) => OnChanged();

    void OnChanged()
    {
        Refresh();

        // cập nhật info theo ô đang được chọn
        if (!inv || infoPanel == null) return;

        int i = inv.selected;
        if (i < 0 || i >= inv.hotbar.Length) return;

        var st = inv.hotbar[i];
        infoPanel.ShowItem(st.item);
    }

    // click vào ô hotbar
    public void OnClickSlot(int i)
    {
        inv?.SelectSlot(i);
        // Không gọi ShowItem ở đây,
        // vì SelectSlot sẽ bắn event SelectedChanged → OnChanged() chạy, tự update info.
    }

    // click chuột phải lên ô hotbar để dùng vật phẩm tiêu hao
    public void OnRightClickSlot(int i)
    {
        if (!inv) return;
        inv.SelectSlot(i);
        if (TrySellInShop(i)) return;
        if (!consumableUser && inv) consumableUser = inv.GetComponent<PlayerUseConsumable>();
        consumableUser?.TryUseSelected(ignoreUiGuard: true);
    }
    bool TrySellInShop(int hotbarIndex)
    {
        if (!vendorShopUI || !vendorShopUI.IsVisible || vendorShopUI.CurrentVendor == null) return false;
        if (!economy) return false;
        if ((uint)hotbarIndex >= (uint)inv.hotbar.Length) return false;

        var st = inv.hotbar[hotbarIndex];
        if (st.item == null || st.item.category != ItemCategory.Equipment) return false;

        int vendorPrice = GetVendorPriceForItem(st.item, vendorShopUI.CurrentVendor);
        if (vendorPrice < 0) return false;

        int sellPrice = Mathf.CeilToInt(vendorPrice / 3f);
        if (sellPrice <= 0) return false;

        return economy.TrySell(st.item, Mathf.Max(1, st.count), out _, sellPrice);
    }

    int GetVendorPriceForItem(ItemSO item, EquipmentVendor vendor)
    {
        if (item == null || vendor == null) return -1;

        var stock = vendor.Stock;
        if (stock != null)
        {
            for (int i = 0; i < stock.Count; i++)
            {
                if (stock[i].item == item)
                {
                    return stock[i].GetPrice();
                }
            }
        }

        return item.buyPrice;
    }

    public void Refresh()
    {
        if (!inv || slots == null) return;
        int n = Mathf.Min(slots.Length, inv.hotbar.Length);
        for (int i = 0; i < n; i++)
        {
            var st = inv.hotbar[i];
            slots[i]?.Render(st, i == inv.selected, i, this);
        }
        for (int i = n; i < slots.Length; i++) slots[i]?.Render(default, false, i, this);
    }
    
    public void RequestSwap(int a, int b)
    {
        if (!inv) return;
        inv.SwapHotbarSlot(a, b);
        Refresh();
    }
    public void RequestMoveOrMerge(int a, int b)
    {
        if (!inv) return;
        inv.MoveOrMergeHotbarSlot(a, b);
        Refresh();
    }
    public void RequestMoveHotbarToBag(int hotbarIndex, int bagIndex)
    {
        if (!inv) return;
        inv.MoveOrSwapHotbarBag(hotbarIndex, bagIndex);
        Refresh(); // cập nhật hotbar

        // Bag UI sẽ tự Refresh nếu đang lắng nghe BagChanged
        // (InventoryBookUI.OnEnable đã sub event BagChanged rồi)
    }
}
