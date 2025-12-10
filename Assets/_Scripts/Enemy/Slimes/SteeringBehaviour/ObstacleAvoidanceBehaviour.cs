using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : SteeringBehaviour
{
    [SerializeField] private float radius = 2f, agentColliderSize = 0.6f;

    [SerializeField] private bool showGizmo = true;

    float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest)
        GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider2D obstacleCollider in aiData.obstacles)
        {
            Vector2 selfPos = transform.position;
            Vector2 obstaclePoint = obstacleCollider.ClosestPoint(selfPos);

            Vector2 dirToObstacle = obstaclePoint - selfPos;
            float dist = dirToObstacle.magnitude;

            // **Trọng số của vật cản**
            float weight;
            if (dist <= agentColliderSize)
                weight = 1f;
            else
                weight = Mathf.Clamp01((radius - dist) / radius);

            if (weight <= 0f)
                continue;

            Vector2 dirNorm = dirToObstacle.normalized;

            // **Cập nhật từng hướng danger**
            for (int i = 0; i < danger.Length; i++)
            {
                Vector2 dir = Directions.eightDirections[i];

                // dot > 0 = obstacle nằm phía hướng đó
                float dot = Vector2.Dot(dirNorm, dir);

                if (dot > 0f)
                {
                    float dangerValue = dot * weight;

                    if (dangerValue > danger[i])
                        danger[i] = dangerValue;
                }
            }
        }

        dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo == false) return;

        if (Application.isPlaying && dangersResultTemp != null)
        {
            if (dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * dangersResultTemp[i]*2);
                }
            }
        }
    }
}

public static class Directions
    {
        public static List<Vector2> eightDirections = new List<Vector2>
        {
            new Vector2(0,1).normalized,
            new Vector2(1,1).normalized,
            new Vector2(1,0).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,1).normalized
        };
    }

