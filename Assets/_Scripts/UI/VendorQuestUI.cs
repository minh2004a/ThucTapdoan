using UnityEngine;
using UnityEngine.UI;

public class VendorQuestUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] GameObject root;          // Panel VendorQuestDialog
    [SerializeField] TypewriterText typewriter;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    EquipmentVendor currentVendor;

    void Awake()
    {
        if (!root)
            root = gameObject;

        root.SetActive(false);   // ẩn sẵn
    }

    public void Show(EquipmentVendor vendor, string message)
    {
        currentVendor = vendor;

        // bật UI
        root.SetActive(true);

        // cho chữ chạy
        if (typewriter != null)
            typewriter.ShowText(message);

        // clear listener cũ để khỏi bị cộng dồn
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(OnClickYes);
        noButton.onClick.AddListener(OnClickNo);
    }

    void Close()
    {
        root.SetActive(false);
        currentVendor = null;
    }

    void OnClickYes()
    {
        if (currentVendor != null)
            currentVendor.OnQuestAnswer(true);

        Close();
    }

    void OnClickNo()
    {
        if (currentVendor != null)
            currentVendor.OnQuestAnswer(false);

        Close();
    }
}
