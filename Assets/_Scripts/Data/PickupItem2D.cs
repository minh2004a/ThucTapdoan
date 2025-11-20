// PickupItem2D.cs
using System.Collections;
using UnityEngine;
// Quản lý vật phẩm có thể nhặt được trong thế giới 2D
public class PickupItem2D : MonoBehaviour
{
    [SerializeField] SpriteRenderer iconRenderer;
    [SerializeField] ItemSO item;
    [SerializeField] int count = 1;

    [Header("Drop settling")]
    [SerializeField, Range(0f, 1f)] float impactDamping = 0.35f;   // giảm lực bật lại sau va chạm
    [SerializeField] float settleDeceleration = 8f;                // lực ma sát khiến vật dừng lại
    [SerializeField] float stopSpeed = 0.05f;                      // tốc độ coi như đã đứng yên
    [SerializeField] int settleFrames = 12;                         // số frame áp lực ma sát sau khi chạm đất
    [Header("Arc flight")]
    [SerializeField] AnimationCurve heightCurve = new(
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(0.5f, 1f, 0f, 0f),
        new Keyframe(1f, 0f, 0f, 0f));
    [SerializeField, Tooltip("Trọng lực giả lập cho quỹ đạo bay lên rồi rơi xuống.")]
    float arcGravity = -18f;
    [SerializeField, Tooltip("Thời gian bay tối thiểu để kịp thấy quỹ đạo.")]
    float minFlightTime = 0.12f;

    Rigidbody2D body;
    int settleTicksRemaining;
    bool inFlight;
    Coroutine flightRoutine;
    Transform visual;

    void Reset()
    {
        iconRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Awake()
    {
        if (!iconRenderer) iconRenderer = GetComponentInChildren<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        visual = iconRenderer ? iconRenderer.transform : transform;
    }

    void OnEnable()
    {
        inFlight = false;
        settleTicksRemaining = 0;
        if (visual && visual != transform) visual.localPosition = Vector3.zero;
    }

    public void Set(ItemSO i, int c)
    {
        item = i;
        count = Mathf.Max(1, c);
        if (iconRenderer) iconRenderer.sprite = i ? i.icon : null;
    }

    public void Launch(Vector2 velocity, float arcHeight, float flightTime)
    {
        if (body)
        {
            body.velocity = velocity;
        }

        if (flightRoutine != null)
            StopCoroutine(flightRoutine);

        flightRoutine = StartCoroutine(FlightRoutine(arcHeight, Mathf.Max(minFlightTime, flightTime)));
    }

    void FixedUpdate()
    {
        if (!body) return;
        if (settleTicksRemaining <= 0) return;

        settleTicksRemaining--;

        Vector2 v = body.velocity;
        if (v.sqrMagnitude <= stopSpeed * stopSpeed)
        {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            return;
        }

        float maxDelta = settleDeceleration * Time.fixedDeltaTime;
        body.velocity = Vector2.MoveTowards(v, Vector2.zero, maxDelta);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!body) return;
        if (collision.collider.isTrigger) return;

        settleTicksRemaining = Mathf.Max(settleTicksRemaining, settleFrames);

        if (impactDamping <= 0f || impactDamping >= 1f) return;
        Vector2 v = body.velocity;
        if (v.sqrMagnitude <= 0f) return;
        body.velocity = v * (1f - impactDamping);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (inFlight) return; // chưa rơi xuống đất thì chưa nhặt
        if (!other.CompareTag("Player")) return;
        var inv = other.GetComponent<PlayerInventory>(); if (!inv) return;

        var result = inv.AddItemDetailed(item, count);
        if (result.remaining <= 0) Destroy(gameObject);
        else count = result.remaining; // kho còn dư chỗ → giữ lại phần chưa nhét được
    }

    IEnumerator FlightRoutine(float arcHeight, float duration)
    {
        inFlight = true;

        float t = 0f;
        Vector3 basePos = visual ? visual.localPosition : Vector3.zero;
        var curve = heightCurve == null || heightCurve.length == 0
            ? AnimationCurve.Linear(0, 1, 1, 1)
            : heightCurve;

        // Tính vận tốc ban đầu và gia tốc để vật đạt độ cao arcHeight rồi rơi xuống trong "duration" với cảm giác trọng lực
        float gravity = arcGravity;
        if (Mathf.Approximately(arcGravity, 0f)) gravity = -18f; // fallback để tránh chia cho 0

        // Với g âm, tính vận tốc ban đầu để đạt đỉnh ở t = duration / 2
        // v0 = g * (duration / 2) * -1 để vận tốc tại đỉnh = 0
        float halfTime = duration * 0.5f;
        float v0 = -gravity * halfTime;

        // Điều chỉnh g và v0 để bảo đảm độ cao đạt gần arcHeight
        // H = v0^2 / (2 * -g) ≈ arcHeight => scale cả hai nếu cần
        float desiredHeight = arcHeight <= 0f ? 0.01f : arcHeight;
        float currentHeight = (v0 * v0) / (2f * -gravity);
        if (currentHeight > 0.001f)
        {
            float scale = desiredHeight / currentHeight; // scale đồng thời để giữ thời gian bay nhưng đổi độ cao
            v0 *= scale;
            gravity *= scale;
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            float height = v0 * t + 0.5f * gravity * t * t;
            if (height <= 0f && t > 0f) break; // chạm đất

            // Cho phép chỉnh profile (ví dụ cong đầu/cuối) nhưng vẫn dựa trên quỹ đạo trọng lực
            height *= curve.Evaluate(normalized);

            if (visual) visual.localPosition = basePos + Vector3.up * height;
            yield return null;
        }

        if (visual) visual.localPosition = basePos;
        inFlight = false;
    }
}