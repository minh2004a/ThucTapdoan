// PickupItem2D.cs
using UnityEngine;
// Quản lý vật phẩm có thể nhặt được trong thế giới 2D
public class PickupItem2D : MonoBehaviour
{
    [SerializeField] SpriteRenderer iconRenderer;
    [SerializeField] ItemSO item;
    [SerializeField] int count = 1;
    [Header("Hiệu ứng bay parabol")]
    [Tooltip("Thời gian item bay trên không")]
    [SerializeField] float flightDuration = 0.5f;
    [Tooltip("Độ cao tối đa của quỹ đạo bay")]
    [SerializeField] float arcHeight = 1.5f;
    [Header("Hiệu ứng nảy sau khi đáp")]
    [SerializeField] int minBounces = 1;        // số lần nảy ít nhất
    [SerializeField] int maxBounces = 3;        // số lần nảy nhiều nhất
    [SerializeField] float firstBounceHeight = 0.4f;  // độ cao cú nảy đầu
    [SerializeField] float bounceHeightDamp = 0.5f;   // mỗi lần nảy sau thấp hơn
    [SerializeField] float bounceDuration = 0.18f;    // thời gian cho 1 cú nảy
    [SerializeField] float groundJitter = 0.05f;      // xê dịch nhẹ trên mặt đất
    bool isFlying;
    bool canPickup = true;
    float flightTimer;
    Vector2 startPos;
    Vector2 travelDir;
    float travelDistance;
    bool isBouncing;
    int remainingBounces;
    float bounceTimer;
    float currentBounceHeight;
    Vector2 groundPos;
    Rigidbody2D body;

    void Reset()
    {
        iconRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Awake()
    {
        if (!iconRenderer) iconRenderer = GetComponentInChildren<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // ----- PHASE BAY PARABOL -----
        if (isFlying)
        {
            flightTimer += Time.deltaTime;
            float t = Mathf.Clamp01(flightTimer / flightDuration);

            // di chuyển ngang: từ 0 -> travelDistance theo t
            Vector2 horizontal = travelDir * (travelDistance * t);

            // “chiều cao” fake: parabol 0 -> arcHeight -> 0
            float h = 4f * arcHeight * t * (1f - t);

            // dịch vị trí: ground + ngang + cao (cao = dịch theo Vector2.up)
            transform.position = startPos + horizontal + Vector2.up * h;

            if (t >= 1f)
            {
                // đáp đất xong -> chuyển sang phase NẢY
                isFlying = false;
                StartBounce();
            }

            return;
        }

        // ----- PHASE NẢY SAU KHI ĐÁP -----
        if (isBouncing)
        {
            bounceTimer += Time.deltaTime;
            float t = Mathf.Clamp01(bounceTimer / bounceDuration);

            // parabol cho cú nảy hiện tại
            float h = 4f * currentBounceHeight * t * (1f - t);

            // giữ item quanh vị trí groundPos, chỉ dịch lên xuống
            transform.position = groundPos + Vector2.up * h;

            if (t >= 1f)
            {
                remainingBounces--;

                if (remainingBounces <= 0)
                {
                    // hết nảy -> đứng yên, cho nhặt
                    isBouncing = false;
                    transform.position = groundPos;
                    canPickup = true;
                }
                else
                {
                    // chuẩn bị cú nảy tiếp theo (thấp hơn + lệch nhẹ)
                    groundPos += Random.insideUnitCircle * groundJitter;
                    currentBounceHeight *= bounceHeightDamp;
                    bounceTimer = 0f;
                }
            }
        }
    }
    void OnEnable()
    {
        if (body)
        {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.gravityScale = 0f;
        }

        isFlying = false;
        isBouncing = false;
        canPickup = true;
    }
    public void Set(ItemSO i, int c)
    {
        item = i;
        count = Mathf.Max(1, c);
        if (iconRenderer) iconRenderer.sprite = i ? i.icon : null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canPickup) return;                 // <<< thêm dòng này
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInventory>(); 
        if (!inv) return;

        var result = inv.AddItemDetailed(item, count);
        if (result.remaining <= 0) Destroy(gameObject);
        else count = result.remaining;
    }
    public void Launch(Vector2 dir, float distance)
    {
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right; // phòng trường hợp dir = (0,0)

        startPos = transform.position;
        travelDir = dir.normalized;
        travelDistance = distance;
        flightTimer = 0f;

        isFlying = true;
        isBouncing = false;
        canPickup = false;   // đang bay thì chưa nhặt được
    }
    void StartBounce()
    {
        groundPos = transform.position;
        remainingBounces = Random.Range(minBounces, maxBounces + 1);
        currentBounceHeight = firstBounceHeight;
        bounceTimer = 0f;

        if (remainingBounces > 0)
        {
            isBouncing = true;
            // vẫn chưa cho nhặt, đợi nảy xong
            canPickup = false;
        }
        else
        {
            // nếu config = 0 nảy thì cho nhặt luôn
            isBouncing = false;
            canPickup = true;
        }
    }
}