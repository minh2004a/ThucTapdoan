using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] int money;

    public int Money => money;

    public event Action<int> MoneyChanged;

    public void SetMoney(int amount)
    {
        int clamped = Mathf.Max(0, amount);
        if (clamped == money) return;

        money = clamped;
        MoneyChanged?.Invoke(money);
    }

    public void AddMoney(int amount)
    {
        if (amount == 0) return;
        SetMoney(money + amount);
    }

    public bool CanAfford(int cost)
    {
        return cost >= 0 && money >= cost;
    }

    public bool TrySpend(int cost)
    {
        if (!CanAfford(cost)) return false;
        SetMoney(money - cost);
        return true;
    }
}
