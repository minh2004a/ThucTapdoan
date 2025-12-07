using UnityEngine;

// Trạng thái tấn công cho Slime
// SlimeController chuyển sang trạng thái này khi ở gần player
public class AttackState : SlimeState
{
    private float attackDuration = 0.5f;
    private float timer;

    // Constructor
    public AttackState(SlimesController slimesController) : base(slimesController) {}

    public override void Enter()
    {
        base.Enter();

        timer = 0f;

        controller.animator.SetTrigger("Attack");

        controller.lastAttackTime = Time.time;

        controller.moveDirection = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        // Quay về trạng thái truy đuổi sau khi tấn công xong
        if (timer >= attackDuration)
        {
            controller.ChangeState(new ChaseState(controller));
        }
    }
}
