using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehaviour : SteeringBehaviour
{
    [Header("Wander Settings")]
    [SerializeField] private float wanderStrength = 0.5f;  // độ mạnh của wander
    [SerializeField] private float changeRate = 1.2f;      // tốc độ đổi hướng random
    [SerializeField] private bool showGizmo = true;

    private float[] interestTemp;
    private Vector2 wanderDirection = Vector2.zero;

    private float timer = 0f;

    private void Start()
    {
        // Khởi tạo hướng wander random ban đầu
        wanderDirection = Random.insideUnitCircle.normalized;
    }

    public override (float[] danger, float[] interest)
        GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        timer += Time.deltaTime;

        // Đổi hướng random mỗi changeRate giây
        if (timer >= changeRate)
        {
            timer = 0f;

            // Thay đổi hướng một cách nhẹ nhàng
            Vector2 newDir = Random.insideUnitCircle.normalized;

            // Tránh đổi hướng quá gắt
            wanderDirection = Vector2.Lerp(wanderDirection, newDir, 0.5f).normalized;
        }

        // Gán hướng wander vào các interest directions
        for (int i = 0; i < Directions.eightDirections.Count; i++)
        {
            float dot = Vector2.Dot(wanderDirection, Directions.eightDirections[i]);

            if (dot > 0)
            {
                float value = dot * wanderStrength;
                if (value > interest[i])
                    interest[i] = value;
            }
        }

        interestTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        if (!Application.isPlaying || interestTemp == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, wanderDirection * 1.5f);
    }
}
