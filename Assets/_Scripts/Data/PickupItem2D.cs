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

        flightRoutine = StartCoroutine(FlightRoutine(arcHeight, Mathf.Max(0.05f, flightTime)));
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
            ? AnimationCurve.EaseInOut(0, 0, 1, 0)
            : heightCurve;

        while (t < duration)
        {
            float normalized = Mathf.Clamp01(t / duration);
            float offset = arcHeight * curve.Evaluate(normalized);
            if (visual) visual.localPosition = basePos + Vector3.up * offset;
            t += Time.deltaTime;
            yield return null;
        }

        if (visual) visual.localPosition = basePos;
        inFlight = false;
    }
}