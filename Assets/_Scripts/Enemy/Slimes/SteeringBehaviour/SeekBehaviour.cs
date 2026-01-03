using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField] private float targetReachedThreshold = 0.5f;
    [SerializeField] private bool showGizmos = true;

    private bool reachedLastTarget = true;
    private Vector2 targetPositionCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest)
        GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        Vector2 selfPos = (Vector2)transform.position;

        // Không có target -> reset
        if (aiData.targets == null || aiData.targets.Count == 0)
        {
            reachedLastTarget = true;

            if (aiData.allowSeekReset)
                aiData.currentTarget = null;
            return (danger, interest);
        }

        if (aiData.currentTarget == null || !aiData.targets.Contains(aiData.currentTarget))
        {
            reachedLastTarget = true; // force reset
        }

        // Nếu vừa reset chọn target gần nhất
        if (reachedLastTarget)
        {
            reachedLastTarget = false;

            Transform closestTarget = null;
            float closestDist = float.PositiveInfinity;

            if (aiData.targets != null)
            {
                for (int i = 0; i < aiData.targets.Count; i++)
                {
                    Transform t = aiData.targets[i];
                    if (t == null) continue;

                    float distSqr = ((Vector2)t.position -selfPos).sqrMagnitude;
                    if (distSqr < closestDist)
                    {
                        closestDist = distSqr;
                        closestTarget = t;
                    }
                }
            }

            aiData.currentTarget = closestTarget;

            if (aiData.currentTarget == null)
            {
                return (danger, interest);
            }
        }

        // Cached position target
        if (aiData.currentTarget != null)
        {
            targetPositionCached = aiData.currentTarget.position;
        }

        // Nếu tới nơi -> reset
        if (Vector2.Distance(selfPos, targetPositionCached) < targetReachedThreshold)
        {
            reachedLastTarget = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }

        // Viết vào interest bằng Dot
        Vector2 direction = targetPositionCached - selfPos;
        Vector2 dirNormalized = direction.normalized;

        for (int i = 0; i < interest.Length; i++)
        {
            float dot = Vector2.Dot(dirNormalized, Directions.eightDirections[i]);
            if (dot > interest[i] && dot > 0)
            {
                interest[i] = Mathf.Clamp01(dot);
            }
        }

        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPositionCached, 0.2f);

        if (Application.isPlaying && interestsTemp != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < interestsTemp.Length; i++)
            {
                Gizmos.DrawRay(transform.position, 
                               Directions.eightDirections[i] * interestsTemp[i] * 2);
            }

            if (reachedLastTarget == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPositionCached, 0.1f);
            }
        }
    }
}
