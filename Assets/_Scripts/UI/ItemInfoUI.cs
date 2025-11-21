using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI descText;

    public void ShowItem(ItemSO item)
    {
        if (!item)
        {
            // clear panel khi không có item
            if (icon) { icon.enabled = false; icon.sprite = null; }
            if (nameText) nameText.text = "";
            if (typeText) typeText.text = "";
            if (descText) descText.text = "";
            return;
        }

        // Icon
        if (icon)
        {
            icon.sprite = item.icon;
            icon.enabled = item.icon != null;
        }

        // Tên
        if (nameText)
        {
            var displayName = string.IsNullOrWhiteSpace(item.displayName)
                ? item.name     // fallback tên asset
                : item.displayName;
            nameText.text = displayName;
        }

        // Loại
        if (typeText)
        {
            string typeStr = item.category switch
            {
                ItemCategory.Tool       => "Công cụ",
                ItemCategory.Weapon     => "Vũ khí",
                ItemCategory.Resource   => "Tài nguyên",
                ItemCategory.Consumable => "Tiêu hao",
                ItemCategory.Minerals   => "Khoáng sản",
                ItemCategory.Seed       => "Hạt giống",
                ItemCategory.Equipment  => "Trang bị",
                ItemCategory.FarmProduct=> "Nông sản",
                _                       => "Khác"
            };
            typeText.text = typeStr;
        }

        // Mô tả + stats
        if (descText)
        {
            var sb = new StringBuilder();

            // mô tả gốc mình gõ tay
            if (!string.IsNullOrWhiteSpace(item.baseDescription))
                sb.AppendLine(item.baseDescription);

            // ====== EQUIP: mũ / đồ mặc tăng may mắn, máu, stamina,... ======
            if (item.category == ItemCategory.Equipment)
            {
                if (item.dropChanceBonusPercent > 0)
                    sb.AppendLine($"• +{item.dropChanceBonusPercent}% may mắn rơi đồ");

                if (item.staminaMaxBonus > 0)
                    sb.AppendLine($"• +{item.staminaMaxBonus} thể lực tối đa");

                if (item.healthMaxBonus > 0)
                    sb.AppendLine($"• +{item.healthMaxBonus} máu tối đa");

                if (item.staminaRegenBonus > 0)
                    sb.AppendLine($"• +{item.staminaRegenBonus}hồi thể lực/giờ ");

                if (item.backpackSlotBonus > 0)
                    sb.AppendLine($"• Mở thêm {item.backpackSlotBonus} ô túi");
            }

            // ====== CONSUMABLE / NÔNG SẢN: hồi máu & stamina ======
            if (item.IsConsumableType()) // Consumable hoặc FarmProduct:contentReference[oaicite:1]{index=1}
            {
                if (item.healthRestore > 0)
                    sb.AppendLine($"• Hồi {item.healthRestore} máu");

                if (item.staminaRestore > 0)
                    sb.AppendLine($"• Hồi {item.staminaRestore} thể lực");
            }

            // (tuỳ thích: thêm info vũ khí, tool range, damage,...)

            descText.text = sb.ToString().TrimEnd();
        }
    }
}
