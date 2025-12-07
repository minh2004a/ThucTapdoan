using UnityEngine;

public class SteeringBehavior
{
    private Transform self;

    // Wander parameters
    private float wanderRadius = 1f; // bán kính vòng tròn để tạo dao động
    private float wanderDistance = 2f; // điểm mục tiêu nằm cách slime bao xa phía trước
    private float wanderJitter = 0.5f; // độ lắc mỗi frame
    private Vector2 wanderTarget; // điểm mục tiêu ngẫu nhiên ban đầu

    // Debug
    public Vector2 targetLocal;
    public Vector2 targetWorld;
    public Vector2 lastSeekTarget;

    // Constructor
    // Mỗi slime sẽ có một wanderTarget khác nhau, nên mỗi con đi mỗi kiểu, không bao giờ giống nhau.
    public SteeringBehavior(Transform self)
    {
        this.self = self;
        // Initialize wander target
        wanderTarget = Random.insideUnitCircle.normalized * wanderRadius;
    }

    // Wander Steering Behavior
    public Vector2 Wander()
    {
        // Thêm jitter ngẫu nhiên
        wanderTarget += new Vector2(Random.Range(-1f, 1f) * wanderJitter, Random.Range(-1f, 1f) * wanderJitter);

        wanderTarget = wanderTarget.normalized * wanderRadius;

        // Điểm mục tiêu phía trước slime
        targetLocal = wanderTarget + Vector2.up * wanderDistance;
        targetWorld = (Vector2)self.position + targetLocal;

        // Hướng di chuyển
        return (targetWorld - (Vector2)self.position).normalized;
    }

    // Seek Steering Behavior
    public Vector2 Seek(Vector2 targetPosition)
    {
        // Store debug information so Gizmos can visualize the seek target
        lastSeekTarget = targetPosition;
        targetWorld = targetPosition;
        lastSeekTarget = targetPosition;

        return ((Vector2)targetPosition - (Vector2)self.position).normalized;
    }
}
