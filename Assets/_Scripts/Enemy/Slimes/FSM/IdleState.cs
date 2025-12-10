using UnityEngine;

// Trạng thái Idle cho Slime
// SlimeController chuyển sang trạng thái này khi không có gì để làm
// Idle có thể chuyển sang Wander hoặc Chase tùy vào điều kiện
public class IdleState : SlimeState
{
    private float idleTime;
    private float timer;

    // Constructor
    public IdleState(SlimesController slimesController) : base(slimesController) {}

    public override void Enter()
    {
        base.Enter();

        timer = 0f;
        idleTime = Random.Range(1f, 2f);

        controller.moveDirection = Vector2.zero;

        controller.animator.SetFloat("Speed", 0f);

        controller.aiData.currentBehaviours.Clear();
        controller.aiData.currentBehaviours.Add(controller.wanderBehaviour);
    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        // Chuyển sang Attack nếu player trong tầm
        if (Vector2.Distance(controller.body.position, controller.player.position) < 3f)
        {
            controller.ChangeState(new ChaseState(controller));
        }

        // Chuyển sang Wander
        if (timer >= idleTime)
        {
            controller.ChangeState(new WanderState(controller));
        }
    }

    public override void Exit()
    {
        base.Exit();
        controller.moveDirection = Vector2.zero;
    }
}
