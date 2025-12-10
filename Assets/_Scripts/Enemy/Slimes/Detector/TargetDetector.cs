using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private LayerMask obstacleLayer, playerLayerMask;

    [SerializeField] private bool showGizmos = false;
    [SerializeField] public float detectionRadius = 3f;

    private static readonly List<Transform> emptyList = new List<Transform>(0);

    public override void Detect(AIData aiData)
    {
        aiData.targets.Clear();

        // 1. Tìm player trong bán kính
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            playerLayerMask
        );

        if (playerCollider == null)
            return;

        Transform target = playerCollider.transform;

        // 2. Kiểm tra line-of-sight (player có bị che không?)
        Vector2 dir = (target.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, target.position);

        // Raycast CHỈ vào obstacle
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dir,
            dist,
            obstacleLayer
        );

        // Nếu hit VÀ hit không phải player → player bị che
        if (hit.collider != null)
            return;

        // Không bị che → thấy player
        aiData.targets.Add(target);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
