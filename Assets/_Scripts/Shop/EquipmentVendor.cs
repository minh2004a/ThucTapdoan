
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VendorItem
{
    public ItemSO item;
    [Tooltip("Nếu >= 0 sẽ ưu tiên dùng giá này thay cho giá mua gốc của item.")]
    public int priceOverride;
    [Tooltip("Nếu >= 0, cửa hàng sẽ mua lại item này từ người chơi với giá cố định này mỗi cái.")]
    public int playerSellPriceOverride;

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
    public int GetPlayerSellPrice(float multiplier = 0.33f)
    {
        if (playerSellPriceOverride >= 0) return playerSellPriceOverride;
        int basePrice = GetPrice();
        if (basePrice < 0) basePrice = item != null ? item.buyPrice : -1;
        if (basePrice < 0 && item != null) basePrice = item.sellPrice;
        if (basePrice < 0) return -1;
        return Mathf.CeilToInt(basePrice * multiplier);
    }

    public bool HasResourceRequirement => requiredResource != null && requiredResourceAmount > 0;
}
public enum VendorType
{
    Equipment,
    Seed
}
// NPC đứng yên bán trang bị. Khi bấm chuột phải vào sẽ mở bảng shop.
public class EquipmentVendor : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;
    [Header("Loại cửa hàng")]
    [SerializeField] VendorType vendorType = VendorType.Equipment;

    [Header("Cửa hàng trang bị")]
    [SerializeField] List<VendorItem> stock = new List<VendorItem>();
    [SerializeField] VendorShopUI shopUI;
    [SerializeField] float interactDistance = 3f;
    [SerializeField] Transform player;
    public IReadOnlyList<VendorItem> Stock => stock;
    [Header("Quest")]
    [SerializeField] bool useQuest = true; // có dùng quest hay không
    [SerializeField] QuestData quest; // quest để giao cho người chơi
    [SerializeField] bool offerQuestOnFirstTalk = true; // có tự động hỏi nhận quest khi lần đầu nói chuyện không
    [SerializeField] VendorQuestUI questUI; // UI nhiệm vụ

    bool hasOfferedQuest = false;
    bool questAccepted = false;
    bool questCompleted = false;
    public bool QuestAccepted => questAccepted;
    void Reset()
    {
        shopUI = FindObjectOfType<VendorShopUI>(true);
        var pc = FindObjectOfType<PlayerController>(true);
        if (pc) player = pc.transform;

        if (!questUI)
            questUI = FindObjectOfType<VendorQuestUI>(true);
    }
    void Awake()
    {
        if (!shopUI) shopUI = FindObjectOfType<VendorShopUI>(true);

        if (!player)
        {
            var pc = FindObjectOfType<PlayerController>(true);
            if (pc) player = pc.transform;
        }

        if (!questUI)
            questUI = FindObjectOfType<VendorQuestUI>(true);
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

        // Nếu không dùng quest hoặc không gán quest / QuestManager chưa tồn tại -> mở shop như cũ
        var qm = QuestManager.Instance;
        if (!useQuest || quest == null || qm == null)
        {
            OpenShop();
            return;
        }

        // Hỏi trạng thái nhiệm vụ từ QuestManager
        var state = qm.GetState(quest);

        // Chưa nhận nhiệm vụ
        if (state == QuestState.NotAccepted)
        {
            if (offerQuestOnFirstTalk)
            {
                ShowQuestOffer();
                return;
            }

            OpenShop();
            return;
        }

        // Đã nhận rồi -> xem thử đủ đồ để trả chưa
        if (qm.CanTurnIn(quest))
        {
            ShowQuestComplete();
            return;
        }

        // Đã nhận nhưng chưa đủ đồ hoặc đã complete -> mở shop bình thường
        OpenShop();
    }
        void ShowQuestOffer()
    {
        if (!questUI)
            questUI = FindObjectOfType<VendorQuestUI>(true);

        if (questUI != null)
        {
            questUI.ShowOffer(this, quest);
        }
        else
        {
            Debug.LogWarning($"Vendor {name}: không tìm thấy VendorQuestUI, mở shop luôn.");
            OpenShop();
        }
    }

    void ShowQuestComplete()
    {
        if (!questUI)
            questUI = FindObjectOfType<VendorQuestUI>(true);

        if (questUI != null)
        {
            questUI.ShowComplete(this, quest);
        }
        else
        {
            Debug.LogWarning($"Vendor {name}: không tìm thấy VendorQuestUI, mở shop luôn.");
            OpenShop();
        }
    }
        // Người chơi trả lời bảng "nhận nhiệm vụ?"
    public void OnQuestOfferAnswer(bool accept)
    {
        var qm = QuestManager.Instance;

        if (accept && qm != null && quest != null)
        {
            qm.AcceptQuest(quest);
        }

        // Dù Yes hay No thì sau đó mở shop
        OpenShop();
    }

    // Người chơi bấm Yes ở bảng "hoàn thành nhiệm vụ"
    public void OnQuestCompleteConfirmed()
    {
        var qm = QuestManager.Instance;

        if (qm != null && quest != null)
        {
            qm.TryTurnIn(quest);   // trừ đồ + thưởng + set Completed
        }

        // Sau khi nhận thưởng thì mở shop
        OpenShop();
    }
    public void OnQuestAnswer(bool accept)
    {
        questAccepted = accept;

        if (accept)
        {
            // Người chơi ĐỒNG Ý -> lần sau KHÔNG hỏi lại nữa
            hasOfferedQuest = true;
            Debug.Log($"Vendor {name}: player đã NHẬN quest.");
            questCompleted = false;   // đang làm quest
        }
        else
        {
            // Người chơi BẤM NO -> coi như từ chối tạm thời
            // Lần sau nói chuyện -> vẫn cho hiện lại quest
            hasOfferedQuest = false;
            Debug.Log($"Vendor {name}: player từ chối quest.");
        }

        // Tạm thời vẫn mở shop sau khi trả lời
        OpenShop();
    }
    public void OpenShopFromQuestUI()
    {
        OpenShop();
    }
    void ShowQuestDialogueTemp()
    {
        hasOfferedQuest = true;  // đánh dấu là đã hỏi 1 lần

        // Sau này sẽ gọi UI yes/no, giờ test tạm bằng log
        Debug.Log($"[Vendor {name}] ĐANG HỎI QUEST LẦN ĐẦU");

        // Giả lập: tạm coi như player bấm No → mở shop luôn
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
        // 1) Nếu item nằm trong stock và có override → dùng override
        if (stock != null)
        {
            foreach (var v in stock)
            {
                if (v.item == item)
                {
                    // luôn dùng override hoặc base giá không nhân % nữa
                    if (v.playerSellPriceOverride >= 0)
                        return v.playerSellPriceOverride;

                    // fallback: dùng item.sellPrice
                    return item.sellPrice;
                }
            }
        }

        // 2) Nếu không nằm trong stock → dùng item.sellPrice
        return item.sellPrice;
    }
}
