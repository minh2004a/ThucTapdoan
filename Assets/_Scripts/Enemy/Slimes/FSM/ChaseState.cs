using Unity.VisualScripting;
using UnityEngine;

// Trạng thái truy đuổi cho Slime
// SlimeController chuyển sang trạng thái này khi phát hiện player
public class ChaseState : SlimeState
{
    public float speed = 2f;
    // Constructor
    public ChaseState(SlimesController slimesController) : base(slimesController) {}
    
    public override void Enter()
    {
        base.Enter();
        controller.animator.SetFloat("Speed", speed);

        controller.aiData.allowSeekReset = true;
        controller.aiData.currentBehaviours.Clear();
        controller.aiData.currentBehaviours.Add(controller.seekBehaviour);
        controller.aiData.currentBehaviours.Add(controller.avoidanceBehaviour);

    }

    public override void Update()
    {
        base.Update();

        // Lấy hướng đã được ContextSolver tính
        Vector2 moveDir = controller.contextSolver.GetDirectionToMove(
            controller.aiData.currentBehaviours,
            controller.aiData
        );

        controller.moveDirection = moveDir * speed;

        // Animation
        controller.animator.SetFloat("Horizontal", moveDir.x);
        controller.animator.SetFloat("Vertical", moveDir.y);
        controller.spriter.flipX = moveDir.x > 0;

        // Check distance để chuyển state
        float dis = Vector2.Distance(controller.body.position, controller.player.position);

        if (dis <= 2f)
        {
            if (Time.time - controller.lastAttackTime >= controller.attackCooldown)
            {
                controller.ChangeState(new AttackState(controller));
            }
            return;
        }

        if (dis > 5f)
        {
            controller.ChangeState(new IdleState(controller));
        }
    }

    public override void Exit()
    {
        base.Exit();
        controller.aiData.allowSeekReset = false;
        controller.aiData.currentBehaviours.Clear();
    }
    
}
