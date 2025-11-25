using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    [Tooltip("ID duy nhất dùng để lưu / load. KHÔNG được trùng giữa các quest.")]
    public string id;

    [Tooltip("Tên nhiệm vụ, sẽ hiển thị trong UI / quest log.")]
    public string title;

    [TextArea]
    [Tooltip("Mô tả dài của nhiệm vụ (cho quest log, UI chi tiết).")]
    public string description;

    [Tooltip("ID hoặc tên NPC giao nhiệm vụ (dùng cho UI hoặc logic sau này).")]
    public string giverId;

    [Header("Hội thoại NPC")]
    [TextArea]
    [Tooltip("Câu thoại khi NPC đề nghị nhận nhiệm vụ.")]
    public string offerText;

    [TextArea]
    [Tooltip("Câu thoại khi người chơi đã đủ đồ và quay lại trả nhiệm vụ.")]
    public string completeText;

    [TextArea]
    [Tooltip("Câu cảm ơn NGAY SAU khi người chơi bấm Yes nhận nhiệm vụ, trước khi mở shop. Để trống nếu không muốn dùng.")]
    public string acceptThanksText;

    [Header("Yêu cầu (fetch quest)")]
    [Tooltip("Item mà người chơi cần mang tới cho NPC.")]
    public ItemSO requiredItem;

    [Tooltip("Số lượng item cần.")]
    public int requiredAmount = 1;

    [Header("Phần thưởng")]
    [Tooltip("Item thưởng sau khi hoàn thành nhiệm vụ.")]
    public ItemSO rewardItem;

    [Tooltip("Số lượng item thưởng.")]
    public int rewardAmount = 1;
    [Tooltip("Số tiền thưởng thêm khi hoàn thành nhiệm vụ.")]
    public int moneyReward = 0;

    [Header("Tuỳ chọn")]
    [Tooltip("Nếu bật, sau này có thể cho phép reset / làm lại nhiệm vụ này (logic chưa code, dùng cho tương lai).")]
    public bool repeatable = false;
}
