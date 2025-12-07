using UnityEngine;
// Trạng thái đi lang thang cho Slime
// SlimeController chuyển sang trạng thái này sau khi kết thúc Idle
public class WanderState : SlimeState
{
    private Vector2 wanderTarget;
    private float speed = 40f;
    private float reachThreshold = 0.1f;

    // Idle stop time
    private bool waiting = false;
    private float waitTime = 3f;
    private float waitTimer = 2f;

    // Constructor
    public WanderState(SlimesController slimesController) : base(slimesController) {}

    public override void Enter()
    {
        base.Enter();
        this.PickNewTarget();
    }

    public override void Update()
    {
        base.Update();

        controller.animator.SetFloat("Speed", controller.moveDirection.magnitude);

        // Nếu đang chờ đợi thì không di chuyển
        if (waiting)
        {
            controller.moveDirection = Vector2.zero;
            controller.animator.SetFloat("Speed", 0f);

            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waiting = false;
                this.PickNewTarget();
            }
            return;
        }

        // Tính toán hướng di chuyển
        Vector2 dir = (wanderTarget - (Vector2)controller.body.position).normalized;

        // flip sprite dựa trên hướng di chuyển
        controller.spriter.flipX = dir.x > 0;

        // di chuyển Slime
        controller.moveDirection = dir * speed * Time.fixedDeltaTime;

        // neu đến gần điểm đích thì chọn điểm mới
        if (Vector2.Distance(controller.body.position, wanderTarget) <= reachThreshold)
        {
            this.StartWaiting();
        }

        // chuyển sang trạng thái Chase nếu phát hiện player
        if (Vector2.Distance(controller.body.position, controller.player.position) < 3f)
        {
            controller.ChangeState(new ChaseState(controller));
        }
    }

    public void PickNewTarget()
    {
        // Chọn điểm ngẫu nhiên trong bán kính 3 đơn vị từ vị trí hiện tại
        wanderTarget = (Vector2)controller.body.position + Random.insideUnitCircle * 3f;

        // cập nhật animation
        Vector2 dirAnim = (wanderTarget - (Vector2)controller.body.position).normalized;
        controller.animator.SetFloat("Horizontal", dirAnim.x);
        controller.animator.SetFloat("Vertical", dirAnim.y);
        controller.animator.SetFloat("Speed", dirAnim.magnitude);
    }

    private void StartWaiting()
    {
        waiting = true;
        waitTimer = 0f;
        waitTime = Random.Range(2f, 4f);

        controller.moveDirection = Vector2.zero;
        controller.animator.SetFloat("Speed", 0f);
    }

    public override void Exit()
    {
        base.Exit();
        controller.moveDirection = Vector2.zero;
        controller.animator.SetFloat("Speed", 0f);
    }
}
