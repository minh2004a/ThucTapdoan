using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OreSpawnZone : MonoBehaviour
{
    [System.Serializable]
    public struct OrePrefabData
    {
        public GameObject prefab;
        public float weight;
    }

    [Header("Spawn Config")]
    public OrePrefabData[] orePrefabs;
    public int targetOreCount = 12;

    [Header("References")]
    public EdgeCollider2D spawnArea;

    private List<GameObject> aliveOres = new List<GameObject>();
    private TimeManager timeManager;

    private void Start()
    {
        SpawnInitial();

        timeManager = FindObjectOfType<TimeManager>();
        if (timeManager != null)
            timeManager.OnNewDay += RespawnMissingOres;
    }

    private void OnDestroy()
    {
        if (timeManager != null)
            timeManager.OnNewDay -= RespawnMissingOres;
    }

    // -----------------------
    // INITIAL SPAWN
    // -----------------------
    void SpawnInitial()
    {
        for (int i = 0; i < targetOreCount; i++)
            SpawnOneOre();
    }

    // -----------------------
    // RESPAWN WHEN NEW DAY
    // -----------------------
    void RespawnMissingOres()
    {
        CleanOreList();

        int missing = targetOreCount - aliveOres.Count;
        if (missing <= 0) return;

        for (int i = 0; i < missing; i++)
            SpawnOneOre();
    }

    void CleanOreList()
    {
        aliveOres.RemoveAll(o => o == null);
    }

    // -----------------------
    // SPAWN 1 ORE
    // -----------------------
    void SpawnOneOre()
    {
        GameObject prefab = GetRandomPrefab();

        Vector2 pos = GetRandomPointInsideEdge(spawnArea);

        // tránh spawn chồng
        if (Physics2D.OverlapCircle(pos, 0f, LayerMask.GetMask("Ore")) != null)
        {
            // thử lại
            SpawnOneOre();
            return;
        }

        GameObject ore = Instantiate(prefab, pos, Quaternion.identity);
        aliveOres.Add(ore);
    }

    // -----------------------
    // RANDOM PREFAB BY WEIGHT
    // -----------------------

    Vector2[] GetClosedPolygon(EdgeCollider2D edge)
    {
        Vector2[] pts = edge.points;
        Vector2[] closed = new Vector2[pts.Length + 1];
        for (int i = 0; i < pts.Length; i++)
            closed[i] = pts[i];
        closed[pts.Length] = pts[0]; // kết nối điểm cuối với điểm đầu (tạo hình kín)
        return closed;
    }
    GameObject GetRandomPrefab()
    {
        float total = 0;
        foreach (var o in orePrefabs) total += o.weight;

        float r = Random.value * total;

        foreach (var o in orePrefabs)
        {
            if (r < o.weight)
                return o.prefab;

            r -= o.weight;
        }

        return orePrefabs[0].prefab;
    }

    // -----------------------
    // RANDOM POINT INSIDE EDGE COLLIDER
    // -----------------------
    Vector2 GetRandomPointInsideEdge(EdgeCollider2D edge)
    {
        Bounds b = edge.bounds;
        Vector2[] polygon = GetClosedPolygon(edge);

        // thử tối đa 50 lần để tìm điểm hợp lệ
        for (int i = 0; i < 50; i++)
        {
            Vector2 random = new Vector2(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y)
            );

            if (IsPointInsidePolygon(random, polygon, edge.transform))
                return random;
        }

        // fallback nếu không tìm được
        return edge.transform.TransformPoint(polygon[0]);
    }

    // -----------------------
    // CHECK POINT INSIDE POLYGON (EDGE COLLIDER)
    // -----------------------
    bool IsPointInsidePolygon(Vector2 point, Vector2[] poly, Transform t)
    {
        int crossings = 0;
        int count = poly.Length - 1; // vì đã đóng kín

        for (int i = 0; i < count; i++)
        {
            Vector2 a = t.TransformPoint(poly[i]);
            Vector2 b = t.TransformPoint(poly[i + 1]);

            bool cond = (a.y > point.y) != (b.y > point.y);
            float xCross =
                (b.x - a.x) * (point.y - a.y) / (b.y - a.y + 0.0001f) + a.x;

            if (cond && point.x < xCross)
                crossings++;
        }

        return (crossings % 2 == 1);
        }
}
