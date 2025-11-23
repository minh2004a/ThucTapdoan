using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VendorMaterialCost
{
    public ItemSO item;
    [Min(1)] public int amount;

    public bool IsValid => item != null && amount > 0;
}

[System.Serializable]
public struct VendorItem
{
    public ItemSO item;
    [Tooltip("Nếu >= 0 sẽ ưu tiên dùng giá này thay cho giá mua gốc của item.")]
    public int priceOverride;
    [Tooltip("Tài nguyên cần để mua vật phẩm này (ngoài tiền).")]
    public VendorMaterialCost[] requiredMaterials;

    public int GetPrice()
    {
        if (priceOverride >= 0) return priceOverride;
        if (item == null) return -1;
        return item.buyPrice;
    }
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