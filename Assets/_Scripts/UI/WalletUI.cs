using UnityEngine;
using TMPro;

public class WalletUI : MonoBehaviour
{
    [SerializeField] PlayerWallet wallet;
    [SerializeField] TMP_Text moneyText;

    void Awake()
    {
        if (!wallet)
            wallet = FindObjectOfType<PlayerWallet>(true);

        if (wallet != null)
        {
            wallet.MoneyChanged += OnMoneyChanged;
            OnMoneyChanged(wallet.Money); // sync lần đầu
        }
        else
        {
            Debug.LogWarning("WalletUI: Không tìm thấy PlayerWallet.");
        }
    }

    void OnDestroy()
    {
        if (wallet != null)
            wallet.MoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(int value)
    {
        if (moneyText != null)
        {
            // Hiển thị luôn 7 số, đủ cho 7 ô trong khung
            moneyText.text = value.ToString("0000000");
        }
    }
}
