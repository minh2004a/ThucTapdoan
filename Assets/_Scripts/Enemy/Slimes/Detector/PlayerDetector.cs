using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : Detector
{
    public float radius = 3f;
    public LayerMask playerMask;

    public override void Detect(AIData aiData)
    {
        aiData.targets.Clear();

        Collider2D player = Physics2D.OverlapCircle(transform.position, radius, playerMask);

        if (player != null)
        {
            aiData.targets.Add(player.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
