using UnityEngine;
using UnityEngine.Events;
// Quản lý sức khỏe của người chơi
//Test Git
public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int hp;
     int baseMaxHP;
    public UnityEvent<float> OnHpPercent; // giá trị 0..1

     void Awake()
        {
            baseMaxHP = Mathf.Max(1, maxHP);
            maxHP = baseMaxHP;
            hp = maxHP;
            OnHpPercent?.Invoke(1f);
        }
    public void TakeDamage(int dmg){
        hp = Mathf.Max(0, hp - dmg);
        OnHpPercent?.Invoke((float)hp / maxHP);
    }

    public void Heal(int v)
    {
        hp = Mathf.Min(maxHP, hp + v);
        OnHpPercent?.Invoke((float)hp / maxHP);
    }
    public void SetPercent(float p)
    {
        p = Mathf.Clamp01(p);
        hp = Mathf.RoundToInt(maxHP * p);
        OnHpPercent?.Invoke((float)hp / maxHP);
    }
    public void HealMissingPercent(float p)
    {
        p = Mathf.Clamp01(p);
        int add = Mathf.RoundToInt((maxHP - hp) * p);
        Heal(add);
    }
    public void ApplyMaxBonus(int bonus)
    {
        int oldMax = Mathf.Max(1, maxHP);
        int newMax = Mathf.Max(1, baseMaxHP + Mathf.Max(0, bonus));

        if (oldMax == newMax) return;

        float percent = (float)hp / oldMax;
        maxHP = newMax;
        hp = Mathf.Clamp(Mathf.RoundToInt(percent * maxHP), 0, maxHP);
        OnHpPercent?.Invoke((float)hp / maxHP);
    }
}
