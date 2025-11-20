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
    public PickupItem2D pickupPrefab;
    [Header("Hiệu ứng văng loot")]

    [Tooltip("Khoảng văng tối thiểu của item khi rơi ra")]
    [SerializeField] float flingForceMin = 1.5f;

    [Tooltip("Khoảng văng tối đa của item khi rơi ra")]
    [SerializeField] float flingForceMax = 3.5f;

    [Tooltip("Nếu bật, loot sẽ văng ngược hướng player")]
    [SerializeField] bool flingAwayFromPlayer = true;

    [Tooltip("Độ lệch góc ngẫu nhiên khi văng")]
    [SerializeField] float randomAngle = 25f;

    Transform player;
    PlayerEquipment playerEquipment;
    public List<DropEntry> drops = new();
    void Awake()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerEquipment = playerObj.GetComponent<PlayerEquipment>();
        }
    }
    public void Drop()
    {
        Vector2 pos = transform.position;
        float luckMultiplier = GetDropChanceMultiplier();
        foreach (var d in drops)
        {
            if (!d.item) continue;
            float chance = Mathf.Clamp01(d.chance * luckMultiplier);
            if (Random.value > chance) continue;

            int cnt = Random.Range(d.countRange.x, d.countRange.y + 1);

            if (spawnSingles)
            {
                int pieces = Mathf.Min(cnt, maxSinglesPerDrop);
                for (int k = 0; k < pieces; k++)
                    Spawn(d.item, 1, pos);        // mỗi pickup = 1 cục

                int rest = cnt - pieces;               // phần dư thì gộp 1 cục
                if (rest > 0) Spawn(d.item, rest, pos);
            }
            else
            {
                Spawn(d.item, cnt, pos);          // kiểu cũ: một pickup to
            }
        }
    }

    void Spawn(ItemSO item, int count, Vector2 pos)
    {
        var go = Instantiate(pickupPrefab, pos, Quaternion.identity);
        go.Set(item, count);

        // --- TÍNH HƯỚNG VĂNG ---
        Vector2 dir;

        if (flingAwayFromPlayer && player != null)
        {
            // văng NGƯỢC lại phía player
            dir = (pos - (Vector2)player.position).normalized;
        }
        else
        {
            dir = Random.insideUnitCircle.normalized;
        }

        // xoay random 1 chút cho tự nhiên
        float angle = Random.Range(-randomAngle, randomAngle);
        dir = Quaternion.Euler(0, 0, angle) * dir;

        // khoảng văng (giờ dùng flingForceMin/Max như khoảng cách luôn)
        float distance = Random.Range(flingForceMin, flingForceMax);

        // --- BAY THEO VÒNG CUNG ---
        go.Launch(dir, distance);
    }
    float GetDropChanceMultiplier()
    {
        if (!playerEquipment && player)
        {
            playerEquipment = player.GetComponent<PlayerEquipment>();
        }

        if (playerEquipment)
        {
            return playerEquipment.GetDropChanceMultiplier();
        }

        return 1f;
    }
}