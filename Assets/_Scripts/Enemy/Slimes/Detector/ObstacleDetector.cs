using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : Detector
{
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private LayerMask detectionLayer;

    [SerializeField] private float detectionRad = 2f;
    Collider2D[] colliders;
    public override void Detect(AIData aiData)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, detectionRad, detectionLayer);
        aiData.obstacles = colliders;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;

        if (Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.red;
            foreach(Collider2D obstacleCollider in colliders)
            {
                Gizmos.DrawSphere(obstacleCollider.transform.position, 0.2f);
            }
        }
    }
}
