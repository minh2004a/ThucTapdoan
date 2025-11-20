// DropLootOnDeath.cs
using UnityEngine;
using System.Collections.Generic;
// Xử lý rơi vật phẩm khi đối tượng chết
[System.Serializable]
public class DropEntry
{
    public ItemSO item;
    public Vector2Int countRange = new(1, 1);
    [Range(0, 1)] public float chance = 1f;
}

public class DropLootOnDeath : MonoBehaviour
{
    public bool spawnSingles = true;      // rơi tách từng cục
    public int maxSinglesPerDrop = 12;    // giới hạn số cục sinh ra
    public bool fullCircle = true;        // văng 360°
    public PickupItem2D pickupPrefab;
    public List<DropEntry> drops = new();
    public Vector2 scatterSpeed = new(0.8f, 1.4f);
    public float scatterAngle = 35f;
    [Header("Arc flight")]
    [Tooltip("Độ cao tối đa của quỹ đạo khi văng ra.")]
    public Vector2 launchHeightRange = new(0.65f, 1.35f);
    [Tooltip("Thời gian bay trước khi hạ cánh.")]
    public Vector2 flightDurationRange = new(0.28f, 0.45f);

    Vector2? pendingScatterDir;           // hướng muốn văng (ví dụ ngược hướng player)

    public void SetScatterDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;
        pendingScatterDir = dir.normalized;
    }

    public void Drop()
    {
        if (!pickupPrefab)
        {
            Debug.LogWarning($"{name} chưa gán pickupPrefab cho DropLootOnDeath, bỏ qua rơi loot.");
            return;
        }
        Vector2? dir = pendingScatterDir;
        pendingScatterDir = null;

        Vector2 pos = transform.position;
        foreach (var d in drops)
        {
            if (!d.item) continue;
            if (Random.value > d.chance) continue;

            int cnt = Random.Range(d.countRange.x, d.countRange.y + 1);

            if (spawnSingles)
            {
                int pieces = Mathf.Min(cnt, maxSinglesPerDrop);
                for (int k = 0; k < pieces; k++)
                    Spawn(d.item, 1, pos, dir);        // mỗi pickup = 1 cục

                int rest = cnt - pieces;               // phần dư thì gộp 1 cục
                if (rest > 0) Spawn(d.item, rest, pos, dir);
            }
            else
            {
                Spawn(d.item, cnt, pos, dir);          // kiểu cũ: một pickup to
            }
        }
    }

    void Spawn(ItemSO item, int count, Vector2 pos, Vector2? scatterDir)
    {
        var go = Instantiate(pickupPrefab, pos, Quaternion.identity);
        go.Set(item, count);

        float angDeg;
        if (scatterDir.HasValue)
        {
            float baseDeg = Mathf.Atan2(scatterDir.Value.y, scatterDir.Value.x) * Mathf.Rad2Deg;
            float spread = Mathf.Max(0f, scatterAngle);
            angDeg = spread > 0f
                ? baseDeg + Random.Range(-spread, spread)
                : baseDeg;
        }
        else if (fullCircle)
        {
            angDeg = Random.Range(0f, 360f);
        }
        else
        {
            angDeg = Random.Range(-scatterAngle, scatterAngle);
        }

        float ang = angDeg * Mathf.Deg2Rad;
        Vector2 dir = new(Mathf.Cos(ang), Mathf.Sin(ang));
        float speed = Random.Range(scatterSpeed.x, scatterSpeed.y);
        Vector2 velocity = dir * speed;

        float height = Random.Range(launchHeightRange.x, launchHeightRange.y);
        float time = Random.Range(flightDurationRange.x, flightDurationRange.y);

        go.Launch(velocity, height, time);
    }
}
