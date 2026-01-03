using UnityEngine;
// Trạng thái đi lang thang cho Slime
// SlimeController chuyển sang trạng thái này sau khi kết thúc Idle
public class WanderState : SlimeState
{
    private Vector2 wanderTarget;
    private float speed = 1f;
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

        controller.aiData.currentBehaviours.Clear();
        controller.aiData.currentBehaviours.Add(controller.wanderBehaviour);
        controller.aiData.currentBehaviours.Add(controller.avoidanceBehaviour);

        controller.animator.SetFloat("Speed", 1f);
    }

    public override void Update()
    {
        base.Update();

        // Nếu phát hiện player thì chuyển sang Chase
        if (controller.aiData.currentTarget != null)
        {
            controller.ChangeState(new ChaseState(controller));
            return;
        }

        // steering → context solver → movement
        Vector2 dir = controller.contextSolver.GetDirectionToMove(
            controller.aiData.currentBehaviours,
            controller.aiData
        );

        controller.moveDirection = dir * speed;

        controller.animator.SetFloat("Speed", dir.magnitude);
    }
    public override void Exit()
    {
        base.Exit();
        controller.aiData.currentBehaviours.Clear();
        controller.moveDirection = Vector2.zero;
        controller.animator.SetFloat("Speed", 0f);
    }
}
