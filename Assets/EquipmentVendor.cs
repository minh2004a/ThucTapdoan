
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VendorItem
{
    public ItemSO item;
    [Tooltip("Nếu >= 0 sẽ ưu tiên dùng giá này thay cho giá mua gốc của item.")]
    public int priceOverride;

    [Header("Chi phí tài nguyên đi kèm")]
    [Tooltip("Tài nguyên cần có thêm để mua trang bị này (ngoài tiền).")]
    public ItemSO requiredResource;
    [Tooltip("Số lượng tài nguyên cần có để mua.")]
    public int requiredResourceAmount;

    public int GetPrice()
    {
        if (priceOverride >= 0) return priceOverride;
        if (item == null) return -1;
        return item.buyPrice;
    }

    public bool HasResourceRequirement => requiredResource != null && requiredResourceAmount > 0;
}

// NPC đứng yên bán trang bị. Khi bấm chuột phải vào sẽ mở bảng shop.
public class EquipmentVendor : MonoBehaviour
{
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

    void OnMouseOver()
    {
        Debug.Log("OnMouseOver running on " + gameObject.name);

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click on vendor " + gameObject.name);
            TryOpenShop();
        }
    }

    public void TryOpenShop()
    {
        if (!shopUI || UIInputGuard.BlockInputNow()) return;
        if (!IsInRange()) return;

        shopUI.Show(this, stock);
    }

    bool IsInRange()
    {
        if (!player) return true;
        return Vector2.Distance(player.position, transform.position) <= interactDistance;
    }
}
