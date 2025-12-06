
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// 1 ô shop nằm ngang hiển thị icon trang bị + giá và icon tiền
public class VendorShopItemUI : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Image currencyIcon;
    [Header("Resource Cost UI")]
    [SerializeField] GameObject resourceCostRoot;
    [SerializeField] Image resourceIcon;
    [SerializeField] TextMeshProUGUI resourceAmountText;
    [SerializeField] Button buyButton;
    [SerializeField] LayoutElement layoutElement;

    VendorItem data;
    Action<VendorItem> onClick;
    ItemStack sellStack;
    Action<ItemStack> onSellClick;
    bool isSellMode;

    void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(HandleClick);
        }
    }
    public void Render(VendorItem item, Sprite currency, Action<VendorItem> onClicked)
    {
        data = item;
        onClick = onClicked;
        onSellClick = null;
        isSellMode = false;

        if (layoutElement)
        {
            layoutElement.flexibleWidth = 1f; // để ô nằm ngang chiếm hết chiều ngang
        }

        if (itemIcon)
        {
            itemIcon.sprite = item.item ? item.item.icon : null;
            itemIcon.enabled = item.item && item.item.icon;
        }

        if (itemNameText)
        {
            string name = item.item ? item.item.displayName : "--";
            if (string.IsNullOrWhiteSpace(name) && item.item)
                name = item.item.name;
            itemNameText.text = name;
        }

        if (priceText)
        {
            int price = item.GetPrice();
            priceText.text = price >= 0 ? price.ToString() : "N/A";
        }

        if (currencyIcon)
        {
            currencyIcon.sprite = currency;
            currencyIcon.enabled = currency != null;
        }

        bool hasResourceCost = item.HasResourceRequirement;

        if (resourceCostRoot)
        {
            resourceCostRoot.SetActive(hasResourceCost);
        }

        if (resourceIcon)
        {
            resourceIcon.sprite = hasResourceCost ? item.requiredResource.icon : null;
            resourceIcon.enabled = hasResourceCost && item.requiredResource.icon;
        }

        if (resourceAmountText)
        {
            resourceAmountText.text = hasResourceCost ? item.requiredResourceAmount.ToString() : string.Empty;
        }
    }

    public void RenderSell(ItemStack stack, Sprite currency, int totalPrice, Action<ItemStack> onClicked)
    {
        sellStack = stack;
        onSellClick = onClicked;
        onClick = null;
        isSellMode = true;

        if (layoutElement)
        {
            layoutElement.flexibleWidth = 1f;
        }

        if (itemIcon)
        {
            itemIcon.sprite = stack.item ? stack.item.icon : null;
            itemIcon.enabled = stack.item && stack.item.icon;
        }

        if (itemNameText)
        {
            string name = stack.item ? stack.item.displayName : "--";
            if (string.IsNullOrWhiteSpace(name) && stack.item)
                name = stack.item.name;
            itemNameText.text = name;
        }

        if (priceText)
        {
            priceText.text = $"Sell: {totalPrice}g";
        }

        if (currencyIcon)
        {
            currencyIcon.sprite = currency;
            currencyIcon.enabled = currency != null;
        }

        if (resourceCostRoot) resourceCostRoot.SetActive(false);
        if (resourceIcon)
        {
            resourceIcon.sprite = null;
            resourceIcon.enabled = false;
        }

        if (resourceAmountText)
        {
            resourceAmountText.text = string.Empty;
        }
    }

    void HandleClick()
    {
        if (isSellMode)
        {
            if (sellStack.item == null) return;
            onSellClick?.Invoke(sellStack);
        }
        else
        {
            if (data.item == null) return;
            onClick?.Invoke(data);
        }
    }
}
