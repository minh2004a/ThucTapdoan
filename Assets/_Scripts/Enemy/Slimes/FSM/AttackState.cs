using UnityEngine;

// Trạng thái tấn công cho Slime
// SlimeController chuyển sang trạng thái này khi ở gần player
public class AttackState : SlimeState
{
    private float attackDuration = 0.5f;
    private float timer;
    private bool hasDealtDamage = false;

    // Constructor
    public AttackState(SlimesController slimesController) : base(slimesController) {}

    public override void Enter()
    {
        base.Enter();

        timer = 0f;
        hasDealtDamage = false;

        controller.animator.SetTrigger("Attack");

        controller.lastAttackTime = Time.time;

        controller.moveDirection = Vector2.zero;

        controller.aiData.currentBehaviours.Clear();
    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        // ✅ Gây damage ở giữa animation (0.25s)
        if (!hasDealtDamage && timer >= attackDuration * 0.5f)
        {
            DealDamage();
            hasDealtDamage = true;
        }

        // Quay về trạng thái truy đuổi sau khi tấn công xong
        if (timer >= attackDuration)
        {
            controller.ChangeState(new ChaseState(controller));
        }
    }

    private void DealDamage()
    {
        // Tìm player trong bán kính nhỏ
        Collider2D hit = Physics2D.OverlapCircle(
            controller.transform.position,
            2f, // attack range
            LayerMask.GetMask("Player")
        );

        if (hit != null)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                int damage = controller.GetComponent<EnemyCtrl>().EnemySO.damage;
                player.TakeDamage(damage);
                Debug.Log("Attack dealt damage!");
            }
        }
    }
}
