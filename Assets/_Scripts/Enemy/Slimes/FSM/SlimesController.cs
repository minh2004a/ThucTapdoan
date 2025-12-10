using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Quản lý trạng thái và hành vi của Slime
// Chịu trách nhiệm chuyển đổi giữa các trạng thái khác nhau
// Nhận Instance của từng State
// quản lý con slime, lưu trạng thái hiện tại, gọi Update mỗi frame.
public class SlimesController : MonoBehaviour
{
    [Header("FSM")]
    public SlimeState currentState;
    public Vector2 moveDirection; // hướng di chuyển hiện tại
    public float attackCooldown = 2f;
    [HideInInspector] public float lastAttackTime = -999f;

    [Header("Component References")]
    [SerializeField] public Animator animator;
    [SerializeField] public Transform player;
    [SerializeField] public SpriteRenderer spriter;
    [SerializeField] public Rigidbody2D rb;
    public Transform body;

    [Header("Steering Behavior")]
    public AIData aiData;
    public Detector[] detectors;
    public ContextSolver contextSolver;
    public List<SteeringBehaviour> steering;
    public SeekBehaviour seekBehaviour;
    public ObstacleAvoidance avoidanceBehaviour;
    public WanderBehaviour wanderBehaviour;

    private void Awake()
    {
        if (rb == null)
        {
            rb = body.GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriter == null)
        {
            spriter = GetComponent<SpriteRenderer>();
        }

        if (aiData == null)
        {
            aiData = GetComponent<AIData>();
        }
        
        // Lấy detectors từ object
        if (detectors == null)
        {
            detectors = GetComponentsInChildren<Detector>();
        }

        if (contextSolver == null) 
        {
            contextSolver = GetComponentInChildren<ContextSolver>();
        }

        if (steering == null) 
        {
            steering = GetComponentsInChildren<SteeringBehaviour>().ToList();
        }

        if (seekBehaviour == null) 
        {
            seekBehaviour = GetComponent<SeekBehaviour>();
        }

        if (avoidanceBehaviour == null) 
        {
            avoidanceBehaviour = GetComponent<ObstacleAvoidance>();
        }

        if (wanderBehaviour == null) 
        {
            wanderBehaviour = GetComponent<WanderBehaviour>();
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = moveDirection;
    }
    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        this.RunDetectors();

        if (aiData.targets.Count > 0)
        {
            aiData.currentTarget = aiData.targets[0];
        }
        else
        {
            aiData.currentTarget = null;
        }

        currentState?.Update();
    }

    public void ChangeState(SlimeState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void RunDetectors()
    {
        foreach(var detector in detectors)
        {
            detector.Detect(aiData);
        }
    }
}
