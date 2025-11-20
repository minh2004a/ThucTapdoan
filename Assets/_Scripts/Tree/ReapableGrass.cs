
using UnityEngine;

[RequireComponent(typeof(UniqueId))]
public class ReapableGrass : MonoBehaviour, IReapable
{
    [SerializeField] PickupItem2D pickupPrefab;
    [SerializeField] ItemSO dropItem;
    [SerializeField, Min(0)] int minDrop = 0;
    [SerializeField, Min(0)] int maxDrop = 1;
    [SerializeField] bool fullCircle = true;
    [SerializeField] float scatterAngle = 35f;
    [SerializeField] Vector2 scatterSpeed = new(0.75f, 1.25f);
    [SerializeField] Vector2 launchHeightRange = new(0.45f, 0.9f);
    [SerializeField] Vector2 flightTimeRange = new(0.22f, 0.4f);

    UniqueId uid;
    string sceneName;

    void Awake()
    {
        uid = GetComponent<UniqueId>();
        sceneName = gameObject.scene.IsValid() ? gameObject.scene.name : null;
        if (!uid || string.IsNullOrEmpty(sceneName)) return;

        if (SaveStore.IsGrassReapedInSession(sceneName, uid.Id))
        {
            Destroy(gameObject);
        }
    }

    public void Reap(int damage, Vector2 hitDir, PlayerInventory inv)
    {
        int count = Random.Range(minDrop, maxDrop + 1);

        if (dropItem && count > 0 && pickupPrefab)
        {
            SpawnDrop(hitDir, count);
        }

        if (uid && !string.IsNullOrEmpty(sceneName))
        {
            SaveStore.MarkGrassReapedPending(sceneName, uid.Id);
        }

        Destroy(gameObject);
    }

    void SpawnDrop(Vector2 hitDir, int count)
    {
        var pickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
        pickup.Set(dropItem, count);

        float angDeg;
        if (hitDir.sqrMagnitude > 0.001f)
        {
            float baseDeg = Mathf.Atan2(hitDir.y, hitDir.x) * Mathf.Rad2Deg;
            float spread = Mathf.Max(0f, scatterAngle);
            angDeg = spread > 0f ? baseDeg + Random.Range(-spread, spread) : baseDeg;
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
        Vector2 velocity = dir * Random.Range(scatterSpeed.x, scatterSpeed.y);
        float height = Random.Range(launchHeightRange.x, launchHeightRange.y);
        float time = Random.Range(flightTimeRange.x, flightTimeRange.y);
        pickup.Launch(velocity, height, time);
    }
}
