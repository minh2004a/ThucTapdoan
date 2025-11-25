
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
    [Header("Quest mở đầu")]
    [SerializeField] bool offerQuestOnFirstTalk = true;
    [TextArea]
    [SerializeField] string firstQuestText = "Tôi đang cần tìm cái gì đó... giúp tôi được không?";

    // KHÔNG cần SerializeField nữa, để script tự tìm
    VendorQuestUI questUI;

    bool hasOfferedQuest = false;
    bool questAccepted = false;

    public bool QuestAccepted => questAccepted;
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

        if (offerQuestOnFirstTalk && !hasOfferedQuest)
        {
            ShowQuestDialogue();
        }
        else
        {
            OpenShop();
        }
    }

    void ShowQuestDialogue()
    {
        hasOfferedQuest = true;

        // Nếu chưa có ref thì tự tìm trong scene (kể cả object đang inactive)
        if (questUI == null)
        {
            questUI = FindObjectOfType<VendorQuestUI>(true);
        }

        if (questUI != null)
        {
            questUI.Show(this, firstQuestText);
        }
        else
        {
            Debug.LogWarning($"Vendor {name}: không tìm thấy VendorQuestUI, mở shop luôn.");
            OpenShop();
        }
    }
    public void OnQuestAnswer(bool accept)
    {
        questAccepted = accept;

        if (accept)
        {
            Debug.Log($"Vendor {name}: player đã NHẬN quest.");
            // TODO: sau này nối vào hệ thống nhiệm vụ thật
        }
        else
        {
            Debug.Log($"Vendor {name}: player từ chối quest.");
        }

        // Dù Yes hay No đều mở shop
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
}
