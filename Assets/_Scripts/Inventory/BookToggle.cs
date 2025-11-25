
using UnityEngine;
// Quản lý việc mở/đóng sách kho đồ
public class BookToggle : MonoBehaviour
{
    [SerializeField] GameObject bookPanel; // BookInventoryPanel
    [SerializeField] VendorShopUI vendorShopUI;
    [SerializeField] VendorQuestUI vendorQuestUI;
    float previousTimeScale = 1f;
    bool pausedByBook = false;
    void Awake()
    {
        if (!vendorShopUI) vendorShopUI = FindObjectOfType<VendorShopUI>(true);
    }

    void Update()
    {
        // không mở túi khi đang mở shop hoặc bảng quest
        if ((vendorShopUI && vendorShopUI.IsVisible) ||
            (vendorQuestUI && vendorQuestUI.IsVisible)) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool active = bookPanel.activeSelf;
            bookPanel.SetActive(!active);

            if (!active)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                pausedByBook = true;
            }
            else if (pausedByBook)
            {
                Time.timeScale = previousTimeScale;
                pausedByBook = false;
            }
        }
    }
    public void CloseBook()
    {
        // Nếu sách đang tắt rồi thì khỏi làm gì
        if (!bookPanel || !bookPanel.activeSelf) return;

        bookPanel.SetActive(false);

        if (pausedByBook)
        {
            Time.timeScale = previousTimeScale;
            pausedByBook = false;
        }
    }
    void OnDisable()
    {
        if (pausedByBook)
        {
            Time.timeScale = previousTimeScale;
            pausedByBook = false;
        }
    }
}
