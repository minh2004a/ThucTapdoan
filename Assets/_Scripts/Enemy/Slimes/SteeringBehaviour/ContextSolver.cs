using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField] private bool showGizmos = true;

    private readonly float[] danger = new float[8];
    private readonly float[] interest = new float[8];
    private float[] interestGizmo = new float[8];

    private Vector2 resultDirection = Vector2.zero;
    private float rayLength = 2f;

    public Vector2 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData aiData)
    {
        // Reset arrays
        for (int i = 0; i < 8; i++)
        {
            danger[i] = 0;
            interest[i] = 0;
        }

        // Combine behaviours
        foreach (var behaviour in behaviours)
        {
            behaviour.GetSteering(danger, interest, aiData);
        }

        // Subtract danger
        for (int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        // For gizmo
        interestGizmo = (float[])interest.Clone();

        // Final movement direction
        Vector2 finalDir = Vector2.zero;

        for (int i = 0; i < 8; i++)
        {
            finalDir += Directions.eightDirections[i] * interest[i];
        }

        if (finalDir != Vector2.zero)
            finalDir.Normalize();

        resultDirection = finalDir;

        return resultDirection;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, resultDirection * rayLength);
    }
}
