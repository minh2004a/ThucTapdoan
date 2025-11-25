using UnityEngine;
using UnityEngine.UI;

public class VendorQuestUI : MonoBehaviour
{
    enum QuestDialogMode
    {
        Offer,      // hỏi nhận nhiệm vụ (Yes/No)
        Complete    // báo hoàn thành (chỉ Yes)
    }
    [Header("UI Refs")]
    [SerializeField] GameObject root;          // Panel VendorQuestDialog
    [SerializeField] TypewriterText typewriter;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [Header("Text")]
    [SerializeField, TextArea] string thankYouText = "Cảm ơn đã giúp tôi! Vào shop xem thử nhé.";
    QuestDialogMode mode;
    EquipmentVendor currentVendor;
    bool isInThankYouPhase;
    void Awake()
    {
        if (!root)
            root = gameObject;

        root.SetActive(false);   // ẩn sẵn
    }

    public void Show(EquipmentVendor vendor, string message)
    {
        mode = QuestDialogMode.Offer;
        currentVendor = vendor;

        root.SetActive(true);

        if (typewriter != null)
            typewriter.ShowText(message);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnClickYes);
        noButton.onClick.AddListener(OnClickNo);
        noButton.gameObject.SetActive(true);
    }
    void Close()
    {
        root.SetActive(false);
        currentVendor = null;
    }

    void OnClickYes()
    {
        if (currentVendor == null)
        {
            Close();
            return;
        }

        if (mode == QuestDialogMode.Offer)
        {
            // Nhận quest như cũ
            currentVendor.OnQuestAnswer(true);
            Close();
        }
        else if (mode == QuestDialogMode.Complete)
        {
            // Gọi vendor xử lý thưởng + mở shop
            currentVendor.FinishQuestAndGiveReward();
            Close();
        }
    }
    void OnClickNo()
    {
        if (currentVendor != null)
        {
            currentVendor.OnQuestAnswer(false);
            currentVendor.OpenShopFromQuestUI();
        }

        Close();
    }
    public void ShowQuestCompleted(EquipmentVendor vendor, string message)
    {
        mode = QuestDialogMode.Complete;
        currentVendor = vendor;

        root.SetActive(true);

        if (typewriter != null)
            typewriter.ShowText(message);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnClickYes);
        noButton.gameObject.SetActive(false); // ẩn nút No
    }
}
