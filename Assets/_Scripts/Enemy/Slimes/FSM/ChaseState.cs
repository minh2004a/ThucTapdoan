using UnityEngine;

// Trạng thái truy đuổi cho Slime
// SlimeController chuyển sang trạng thái này khi phát hiện player
public class ChaseState : SlimeState
{
    public float speed = 40f;
    // Constructor
    public ChaseState(SlimesController slimesController) : base(slimesController) {}
    
    public override void Enter()
    {
        base.Enter();
        controller.animator.SetFloat("Speed", speed);
    }

    public override void Update()
    {
        base.Update();

        Vector2 dir = controller.steering.Seek(controller.player.position);
        controller.spriter.flipX = dir.x > 0;

        // Di Chuyển
        controller.moveDirection = dir * speed * Time.fixedDeltaTime;

        controller.animator.SetFloat("Horizontal", dir.x);
        controller.animator.SetFloat("Vertical", dir.y);

        float dis = Vector2.Distance(controller.body.position, controller.player.position);

        // Check cooldown tấn công
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

        // // Chuyển sang trạng thái tấn công nếu trong phạm vi
        // if (Vector2.Distance(controller.body.position, controller.player.position) <= 1f)
        // {
        //     controller.ChangeState(new AttackState(controller));
        // }

        // // Quay về trạng thái patrolling nếu mất player
        // if (Vector2.Distance(controller.body.position, controller.player.position) > 5f)
        // {
        //     controller.ChangeState(new IdleState(controller));
        // }
    }
    
}
