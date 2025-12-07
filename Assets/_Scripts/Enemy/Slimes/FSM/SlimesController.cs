using UnityEngine;

// Quản lý trạng thái và hành vi của Slime
// Chịu trách nhiệm chuyển đổi giữa các trạng thái khác nhau
// Nhận Instance của từng State
// quản lý con slime, lưu trạng thái hiện tại, gọi Update mỗi frame.
public class SlimesController : MonoBehaviour
{
    public SlimeState currentState;
    public SteeringBehavior steering;
    public Vector2 moveDirection; // hướng di chuyển hiện tại
    public Transform body;
    public float attackCooldown = 2f;
    [HideInInspector] public float lastAttackTime = -999f;

    [SerializeField] public Animator animator;
    [SerializeField] public Transform player;
    [SerializeField] public SpriteRenderer spriter;
    [SerializeField] public Rigidbody2D rb;

    private void Awake()
    {
        steering = new SteeringBehavior(this.transform);
        if (rb == null)
        {
            rb = body.GetComponent<Rigidbody2D>();
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = moveDirection * 2f;
    }
    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(SlimeState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void OnDrawGizmos()
    {
        if (steering == null) return;

        // Local wander point
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + steering.targetLocal, 0.1f);

        // World wander target (nơi slime muốn tới)
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(steering.targetWorld, 0.1f);

        // Line hướng tới target
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, steering.targetWorld);

        // Seek target (nếu đang chase player)
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(steering.lastSeekTarget, 0.1f);
    }
}
