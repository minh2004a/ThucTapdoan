
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

    // click chuột trái lên ô hotbar
    public void OnClickSlot(int i, bool allowSell = true)
    {
        if (!inv) return;

        bool wasSelected = inv.selected == i;
        inv.SelectSlot(i);

        // Nếu đang trong shop
        if (vendorShopUI && vendorShopUI.IsVisible)
        {
            // Click lần đầu chỉ chọn, click tiếp trên ô đang chọn mới bán toàn bộ stack
            if (!allowSell)
                return;

            if (wasSelected)
            {
                var st = inv.hotbar[i];
                if (st.item != null)
                    TrySellInShop(i, st.count);
            }
            return;
        }
    }

    // click chuột phải lên ô hotbar
    public void OnRightClickSlot(int i)
    {
        if (!inv) return;

        bool wasSelected = inv.selected == i;
        inv.SelectSlot(i);

        if (vendorShopUI && vendorShopUI.IsVisible)
        {
            // Click lần đầu chỉ chọn, click tiếp trên ô đang chọn mới bán từng cái
            if (wasSelected && TrySellInShop(i, 1))
                return;
            return;
        }

        // ngoài shop hoặc bán fail -> xài consumable như cũ
        if (!consumableUser && inv)
            consumableUser = inv.GetComponent<PlayerUseConsumable>();

        consumableUser?.TryUseSelected(ignoreUiGuard: true);
    }

    bool TrySellInShop(int hotbarIndex, int amount)
    {
        if (!vendorShopUI || !vendorShopUI.IsVisible || vendorShopUI.CurrentVendor == null) return false;
        if (!economy || !inv) return false;
        if ((uint)hotbarIndex >= (uint)inv.hotbar.Length) return false;

        var st = inv.hotbar[hotbarIndex];
        if (st.item == null || !vendorShopUI.CurrentVendor.CanBuyFromPlayer(st.item)) return false;

        int sellPrice = vendorShopUI.CurrentVendor.GetPlayerSellPrice(st.item);
        if (sellPrice <= 0) return false;

        int sellAmount = Mathf.Clamp(amount, 1, st.count);

        return economy.TrySell(st.item, sellAmount, out _, sellPrice);
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
