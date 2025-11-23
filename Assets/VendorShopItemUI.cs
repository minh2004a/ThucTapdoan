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
    [SerializeField] Button buyButton;
    [SerializeField] LayoutElement layoutElement;

    VendorItem data;
    Action<VendorItem> onClick;

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
    }
    void HandleClick()
    {
        if (data.item == null) return;
        onClick?.Invoke(data);
    }
}