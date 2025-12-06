
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Bảng shop hiển thị các ô nằm ngang đầy đủ icon + giá tiền.
public class VendorShopUI : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] BookToggle bookToggle;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] ConsumableConfirmUI confirmUI;
    [Header("Tabs")]
    [SerializeField] Button buyTabButton;
    [SerializeField] Button sellTabButton;
    [Header("UI Refs")]
    [SerializeField] GameObject root;
    [SerializeField] Transform content;
    [SerializeField] VendorShopItemUI itemPrefab;
    [SerializeField] Sprite currencyIcon;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] float feedbackSeconds = 1.5f;
    [SerializeField] Color errorFeedbackColor = Color.red;
    [SerializeField] AudioClip sellSfx;
    [SerializeField] AudioSource sfxSource;
    [Header("Gameplay")]
    [SerializeField] EconomyManager economy;

    enum Mode
    {
        Buy,
        Sell
    }

    readonly List<VendorItem> cachedStock = new List<VendorItem>();
    readonly List<ItemStack> sellableBuffer = new List<ItemStack>();
    readonly List<VendorShopItemUI> pool = new List<VendorShopItemUI>();
    Mode mode = Mode.Buy;
    EquipmentVendor currentVendor;
    Coroutine feedbackRoutine;
    Color feedbackDefaultColor = Color.white;

    public EquipmentVendor CurrentVendor => currentVendor;
    public bool IsVisible => root && root.activeSelf;

    void Awake()
    {
        if (!root) root = gameObject;
        if (!economy) economy = FindObjectOfType<EconomyManager>(true);
        if (!inventory) inventory = FindObjectOfType<PlayerInventory>(true);
        if (!confirmUI) confirmUI = FindObjectOfType<ConsumableConfirmUI>(true);
        if (!sfxSource) sfxSource = GetComponent<AudioSource>();
        SetupTabButtons();
        feedbackDefaultColor = feedbackText ? feedbackText.color : Color.white;
        HideFeedbackImmediate();
        Hide(null);
    }

    public void Show(EquipmentVendor vendor, List<VendorItem> items)
    {
        currentVendor = vendor;
        cachedStock.Clear();
        if (items != null) cachedStock.AddRange(items);
        mode = Mode.Buy;
        UpdateTabVisuals();

        bookToggle?.CloseBook();
        if (root) root.SetActive(true);

        // 🔒 khoá di chuyển
        if (player != null)
            player.SetMoveLock(true);

        RenderCurrentMode();
    }

    public void Hide(EquipmentVendor requester)
    {
        if (requester != null && requester != currentVendor) return;

        currentVendor = null;
        sellableBuffer.Clear();
        HideFeedbackImmediate();

        if (root) root.SetActive(false);

        // 🔓 mở khoá di chuyển
        if (player != null)
            player.SetMoveLock(false);
    }
    public void CloseFromButton()
    {
        Hide(null);
    }

    void RenderCurrentMode()
    {
        if (mode == Mode.Sell)
        {
            RenderSellMode();
        }
        else
        {
            RenderBuyMode();
        }
    }

    void RenderBuyMode()
    {
        if (!itemPrefab || !content) return;

        EnsurePool(cachedStock.Count);
        for (int i = 0; i < pool.Count; i++)
        {
            bool active = i < cachedStock.Count;
            pool[i].gameObject.SetActive(active);
            if (active)
            {
                pool[i].Render(cachedStock[i], currencyIcon, OnBuyItemClicked);
            }
        }
    }

    void RenderSellMode()
    {
        sellableBuffer.Clear();
        if (inventory != null && currentVendor != null)
        {
            sellableBuffer.AddRange(inventory.GetSellableItems(currentVendor));
        }

        if (!itemPrefab || !content) return;

        EnsurePool(sellableBuffer.Count);
        for (int i = 0; i < pool.Count; i++)
        {
            bool active = i < sellableBuffer.Count;
            pool[i].gameObject.SetActive(active);
            if (active)
            {
                var stack = sellableBuffer[i];
                int unitPrice = currentVendor != null ? currentVendor.GetPlayerSellPrice(stack.item) : -1;
                int totalPrice = (unitPrice < 0) ? 0 : unitPrice * stack.count;
                pool[i].RenderSell(stack, currencyIcon, totalPrice, OnSellItemClicked);
            }
        }
    }

    void EnsurePool(int need)
    {
        while (pool.Count < need)
        {
            var entry = Instantiate(itemPrefab, content);
            pool.Add(entry);
        }
    }

    void OnBuyItemClicked(VendorItem item)
    {
        if (economy == null || item.item == null) return;
        UIInputGuard.MarkClick();

        if (!economy.TryBuy(
            item.item,
            1,
            out var _,
            item.GetPrice(),
            item.requiredResource,
            item.requiredResourceAmount))
        {
            ShowFeedback("Không đủ tiền hoặc túi đầy.", true);
        }
    }

    void OnSellItemClicked(ItemStack stack)
    {
        UIInputGuard.MarkClick();
        if (currentVendor == null || inventory == null || economy == null) return;

        if (!currentVendor.CanBuyFromPlayer(stack.item))
        {
            ShowFeedback("Người bán không mua vật phẩm này.", true);
            return;
        }

        int unitPrice = currentVendor.GetPlayerSellPrice(stack.item);
        if (unitPrice < 0)
        {
            ShowFeedback("Người bán không mua vật phẩm này.", true);
            return;
        }

        int totalPrice = unitPrice * stack.count;
        string itemName = GetItemName(stack.item);
        string message = $"Bán {stack.count} cái {itemName} với giá {totalPrice}g?";

        var copy = stack;
        void Confirm() => ConfirmSell(copy, unitPrice);

        if (confirmUI != null)
        {
            confirmUI.Show(stack.item, Confirm, RenderCurrentMode, message);
        }
        else
        {
            ConfirmSell(copy, unitPrice);
        }
    }

    void ConfirmSell(ItemStack stack, int unitPrice)
    {
        if (currentVendor == null || inventory == null || economy == null) return;

        stack.count = Mathf.Max(1, stack.count);
        if (!inventory.HasItem(stack.item, stack.count))
        {
            ShowFeedback("Không đủ vật phẩm để bán.", true);
            RenderCurrentMode();
            return;
        }

        bool sold = currentVendor.TrySellToVendor(stack, inventory, economy);
        if (!sold)
        {
            ShowFeedback("Người bán không mua vật phẩm này.", true);
            RenderCurrentMode();
            return;
        }

        int payout = Mathf.Max(0, unitPrice) * stack.count;
        ShowFeedback($"+{payout}g");
        PlaySellSfx();
        RenderCurrentMode();
    }

    void SetupTabButtons()
    {
        if (buyTabButton)
        {
            buyTabButton.onClick.RemoveAllListeners();
            buyTabButton.onClick.AddListener(OnClickBuyTab);
        }

        if (sellTabButton)
        {
            sellTabButton.onClick.RemoveAllListeners();
            sellTabButton.onClick.AddListener(OnClickSellTab);
        }
    }

    void OnClickBuyTab()
    {
        UIInputGuard.MarkClick();
        if (mode == Mode.Buy) return;
        mode = Mode.Buy;
        UpdateTabVisuals();
        RenderCurrentMode();
    }

    void OnClickSellTab()
    {
        UIInputGuard.MarkClick();
        if (mode == Mode.Sell) return;
        mode = Mode.Sell;
        UpdateTabVisuals();
        RenderCurrentMode();
    }

    void UpdateTabVisuals()
    {
        if (buyTabButton) buyTabButton.interactable = mode != Mode.Buy;
        if (sellTabButton) sellTabButton.interactable = mode != Mode.Sell;
    }

    void ShowFeedback(string message, bool isError = false)
    {
        if (!feedbackText) return;

        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }

        feedbackText.gameObject.SetActive(true);
        feedbackText.color = isError ? errorFeedbackColor : feedbackDefaultColor;
        feedbackText.text = message;
        feedbackRoutine = StartCoroutine(HideFeedbackSoon());
    }

    IEnumerator HideFeedbackSoon()
    {
        float wait = Mathf.Max(0.01f, feedbackSeconds);
        float t = 0f;
        while (t < wait)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        HideFeedbackImmediate();
    }

    void HideFeedbackImmediate()
    {
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }

        if (feedbackText)
        {
            feedbackText.text = string.Empty;
            feedbackText.gameObject.SetActive(false);
            feedbackText.color = feedbackDefaultColor;
        }
    }

    void PlaySellSfx()
    {
        if (sellSfx == null) return;

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(sellSfx);
        }
        else
        {
            var pos = Camera.main ? Camera.main.transform.position : Vector3.zero;
            AudioSource.PlayClipAtPoint(sellSfx, pos);
        }
    }

    string GetItemName(ItemSO item)
    {
        if (!item) return "vật phẩm";
        if (!string.IsNullOrWhiteSpace(item.displayName)) return item.displayName;
        if (!string.IsNullOrWhiteSpace(item.name)) return item.name;
        return "vật phẩm";
    }
}
