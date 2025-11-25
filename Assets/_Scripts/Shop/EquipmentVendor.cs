
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
    [SerializeField] PlayerInventory playerInventory;
    [Header("Cửa hàng trang bị")]
    [SerializeField] List<VendorItem> stock = new List<VendorItem>();
    [SerializeField] VendorShopUI shopUI;
    [SerializeField] float interactDistance = 3f;
    [SerializeField] Transform player;

    public IReadOnlyList<VendorItem> Stock => stock;
    [Header("Quest mở đầu")]
    [SerializeField] bool offerQuestOnFirstTalk = true;
    [Header("Text nhiệm vụ")]
    [TextArea]
    [SerializeField] string firstQuestText = "Tôi đang cần tìm cái gì đó... giúp tôi được không?";
    [TextArea]
    [SerializeField] string thankYouText = "Cảm ơn đã giúp tôi! Vào shop xem thử nhé.";
    [Header("Quest đơn giản")]
    [Tooltip("Người chơi phải mang item này tới.")]
    [SerializeField] ItemSO questRequiredItem;

    [SerializeField] int questRequiredAmount = 10;

    [Header("Thưởng khi hoàn thành")]
    [SerializeField] ItemSO questRewardItem;
    [SerializeField] int questRewardAmount = 1;
    VendorQuestUI questUI;

    bool hasOfferedQuest = false;
    bool questAccepted = false;
    bool questCompleted = false;
    public bool QuestAccepted => questAccepted;
    public string FirstQuestText => firstQuestText;
    public string ThankYouText => thankYouText;
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

        // Tìm PlayerInventory nếu chưa gán
        if (!playerInventory)
        {
            if (player)
                playerInventory = player.GetComponent<PlayerInventory>();

            if (!playerInventory)
                playerInventory = FindObjectOfType<PlayerInventory>(true);
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

        // Nếu đã nhận quest và chưa completed -> thử hoàn thành
        if (questAccepted && !questCompleted)
        {
            // Nếu trả về true nghĩa là đã mở UI hoàn thành -> dừng lại, không mở shop nữa
            if (TryCompleteQuest())
                return;
        }

        // Nếu chưa từng offer quest => show UI nhận nhiệm vụ
        if (offerQuestOnFirstTalk && !hasOfferedQuest)
        {
            ShowQuestDialogue();
        }
        else
        {
            // Còn lại thì mở shop luôn
            OpenShop();
        }
    }
    bool TryCompleteQuest()
    {
        // 1. Đã hoàn thành / chưa nhận thì thôi
        if (!questAccepted || questCompleted) return false;
        if (questRequiredItem == null || questRequiredAmount <= 0) return false;

        // 2. Đảm bảo có inventory
        if (!playerInventory)
        {
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            if (!playerInventory)
            {
                Debug.LogWarning($"Vendor {name}: không tìm thấy PlayerInventory.");
                return false;
            }
        }

        // 3. Check đủ item chưa
        if (!playerInventory.HasItem(questRequiredItem, questRequiredAmount))
        {
            // Có thể log cho debug
            int have = playerInventory.CountItem(questRequiredItem);
            Debug.Log($"Vendor {name}: mới có {have}/{questRequiredAmount}, chưa đủ để hoàn thành quest.");
            return false;
        }

        // 4. Đủ đồ -> mở UI hoàn thành
        if (questUI == null)
            questUI = FindObjectOfType<VendorQuestUI>(true);

        if (questUI != null)
        {
            // tí nữa mình sẽ thêm hàm này trong VendorQuestUI
            questUI.ShowQuestCompleted(this, "Cảm ơn đã mang đồ tới giúp tôi!");
        }

        // CHƯA trừ đồ, CHƯA thưởng ở đây
        // Đợi lúc player bấm YES trong UI mới trừ/ thưởng
        return true;
    }
    void ShowQuestDialogue()
    {
        hasOfferedQuest = true;

        if (questUI == null)
        {
            questUI = FindObjectOfType<VendorQuestUI>(true);
        }

        if (questUI != null)
        {
            // đổi ở đây
            questUI.Show(this, firstQuestText); // hoặc sau này nếu muốn có thể rút gọn còn Show(this)
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
    public void FinishQuestAndGiveReward()
    {
        if (questCompleted) return; // cho chắc, tránh double reward

        if (!playerInventory)
        {
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            if (!playerInventory)
            {
                Debug.LogWarning($"Vendor {name}: không tìm thấy PlayerInventory để phát thưởng.");
                return;
            }
        }

        // 1. Trừ đồ nhiệm vụ
        if (questRequiredItem && questRequiredAmount > 0)
        {
            bool removed = playerInventory.RemoveItem(questRequiredItem, questRequiredAmount);
            if (!removed)
            {
                Debug.LogWarning($"Vendor {name}: lạ nha, lúc nãy đủ đồ mà giờ RemoveItem fail.");
            }
        }

        // 2. Thưởng item
        if (questRewardItem && questRewardAmount > 0)
        {
            int remaining = playerInventory.AddItem(questRewardItem, questRewardAmount);
            if (remaining > 0)
            {
                Debug.LogWarning($"Vendor {name}: túi đầy, còn {remaining} món không add được.");
                // sau này có thể drop xuống đất, gửi vào rương, v.v.
            }
        }

        // 3. Đánh dấu quest đã hoàn thành
        questCompleted = true;
        Debug.Log($"Vendor {name}: QUEST HOÀN THÀNH, đã phát thưởng.");

        // 4. Mở shop như bình thường
        OpenShop();
    }
}
