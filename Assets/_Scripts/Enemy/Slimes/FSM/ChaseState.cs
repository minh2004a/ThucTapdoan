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

        if (controller.aiData.targets.Count == 0 || controller.aiData.currentTarget == null)
        {
            controller.ChangeState(new WanderState(controller));
            return;
        }
        // Lấy hướng đã được ContextSolver tính
        Vector2 moveDir = controller.contextSolver.GetDirectionToMove(
            controller.aiData.currentBehaviours,
            controller.aiData
        );

        float dis = Vector2.Distance(controller.body.position, controller.player.position);

        // Trong attack range VÀ cooldown hết → Attack ngay
        if (dis <= 2f && Time.time - controller.lastAttackTime >= controller.attackCooldown)
        {
            controller.moveDirection = Vector2.zero; // Dừng lại trước khi attack
            controller.ChangeState(new AttackState(controller));
            return;
        }

        controller.moveDirection = moveDir * speed;

        // Animation
        controller.animator.SetFloat("Horizontal", moveDir.x);
        controller.animator.SetFloat("Vertical", moveDir.y);
        controller.spriter.flipX = moveDir.x > 0;
    }

    public override void Exit()
    {
        base.Exit();
        controller.aiData.allowSeekReset = false;
        controller.aiData.currentBehaviours.Clear();
    }
    
}
