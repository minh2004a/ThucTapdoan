using System.Collections.Generic;
using UnityEngine;

// Bảng shop hiển thị các ô nằm ngang đầy đủ icon + giá tiền.
public class VendorShopUI : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [Header("UI Refs")]
    [SerializeField] GameObject root;
    [SerializeField] Transform content;
    [SerializeField] VendorShopItemUI itemPrefab;
    [SerializeField] Sprite currencyIcon;

    [Header("Gameplay")]
    [SerializeField] EconomyManager economy;

    readonly List<VendorShopItemUI> pool = new List<VendorShopItemUI>();
    EquipmentVendor currentVendor;

    public EquipmentVendor CurrentVendor => currentVendor;

    void Awake()
    {
        if (!root) root = gameObject;
        if (!economy) economy = FindObjectOfType<EconomyManager>(true);
        Hide(null);
    }

    public void Show(EquipmentVendor vendor, List<VendorItem> items)
    {
        currentVendor = vendor;

        if (root) root.SetActive(true);

        // 🔒 khoá di chuyển
        if (player != null)
            player.SetMoveLock(true);

        Render(items);
    }
   public void Hide(EquipmentVendor requester)
    {
        if (requester != null && requester != currentVendor) return;

        currentVendor = null;

        if (root) root.SetActive(false);

        // 🔓 mở khoá di chuyển
        if (player != null)
            player.SetMoveLock(false);
    }
    public void CloseFromButton()
    {
        Hide(null);
    }
    void Render(IReadOnlyList<VendorItem> items)
    {
        if (!itemPrefab || !content) return;

        EnsurePool(items.Count);
        for (int i = 0; i < pool.Count; i++)
        {
            bool active = i < items.Count;
            pool[i].gameObject.SetActive(active);
            if (active)
            {
                pool[i].Render(items[i], currencyIcon, OnItemClicked);
            }
        }
    }

    void EnsurePool(int need)
    {
        while (pool.Count < need)
        {
            var entry = Instantiate(itemPrefab, content);
            pool.Add(entry);
        }
    }

    void OnItemClicked(VendorItem item)
    {
        if (economy == null || item.item == null) return;

        if (!economy.TryBuy(
            item.item,
            1,
            out var _,
            item.GetPrice(),
            item.requiredResource,
            item.requiredResourceAmount))
        {
            Debug.Log("Không đủ tiền hoặc túi đầy, không thể mua.");
        }
    }
}