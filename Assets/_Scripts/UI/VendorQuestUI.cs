using UnityEngine;
using UnityEngine.UI;

public class VendorQuestUI : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] TypewriterText typewriter;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    
    enum Mode
    {
        OfferAsk,      // Hỏi: nhận nhiệm vụ không?
        OfferThanks,   // Đã nhận: hiện lời cảm ơn
        Complete       // Đã đủ đồ: hiện hoàn thành nhiệm vụ
    }

    Mode mode;

    EquipmentVendor currentVendor;
    QuestData currentQuest;

    void Awake()
    {
        if (!root) root = gameObject;
        root.SetActive(false);
    }

    // --- Phase 1: hỏi nhận nhiệm vụ ---
    public void ShowOffer(EquipmentVendor vendor, QuestData quest)
    {
        mode = Mode.OfferAsk;
        currentVendor = vendor;
        currentQuest = quest;

        root.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnClickYes);
        noButton.onClick.AddListener(OnClickNo);
        noButton.gameObject.SetActive(true);

        if (typewriter != null)
            typewriter.ShowText(quest != null ? quest.offerText : "");
    }

    // --- Hoàn thành nhiệm vụ ---
    public void ShowComplete(EquipmentVendor vendor, QuestData quest)
    {
        mode = Mode.Complete;
        currentVendor = vendor;
        currentQuest = quest;

        root.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnClickYes);
        noButton.gameObject.SetActive(false); // không có nút No khi trả nhiệm vụ

        if (typewriter != null)
            typewriter.ShowText(quest != null ? quest.completeText : "");
    }

    void OnClickYes()
    {
        if (currentVendor == null)
        {
            Close();
            return;
        }

        switch (mode)
        {
            case Mode.OfferAsk:
                // Người chơi mới bấm YES “nhận nhiệm vụ”
                // Nếu có text cảm ơn -> chuyển sang Phase 2
                if (currentQuest != null &&
                    !string.IsNullOrWhiteSpace(currentQuest.acceptThanksText))
                {
                    mode = Mode.OfferThanks;
                    if (noButton != null)
                        noButton.gameObject.SetActive(false);

                    if (typewriter != null)
                        typewriter.ShowText(currentQuest.acceptThanksText);

                    // CHƯA gọi vendor, chưa mở shop
                    return;
                }
                else
                {
                    // Không set text cảm ơn thì giữ behaviour cũ:
                    // nhận nhiệm vụ + mở shop luôn
                    currentVendor.OnQuestOfferAnswer(true);
                    Close();
                }
                break;

            case Mode.OfferThanks:
                // Đây là lần YES thứ 2 sau khi hiện lời cảm ơn
                currentVendor.OnQuestOfferAnswer(true);
                Close();
                break;

            case Mode.Complete:
                // YES khi trả nhiệm vụ
                currentVendor.OnQuestCompleteConfirmed();
                Close();
                break;
        }
    }

    void OnClickNo()
    {
        if (currentVendor != null && mode == Mode.OfferAsk)
        {
            // Từ chối nhiệm vụ nhưng vẫn mở shop
            currentVendor.OnQuestOfferAnswer(false);
        }

        Close();
    }

    public void Close()
    {
        root.SetActive(false);
        currentVendor = null;
        currentQuest = null;
    }
}
