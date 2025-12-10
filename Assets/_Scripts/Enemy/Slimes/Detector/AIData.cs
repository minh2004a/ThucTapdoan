using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();
    public Collider2D[] obstacles = new Collider2D[0];
    public bool allowSeekReset = false;

    public List<SteeringBehaviour> currentBehaviours = new List<SteeringBehaviour>();
    public Transform currentTarget;

    public void Reset()
    {
        if (targets == null)
            targets = new List<Transform>();
        else 
            targets.Clear();

        obstacles = new Collider2D[0];

        currentTarget = null;
    }

    public int GetTargetsCount()
    {
        return targets == null ? 0 : targets.Count;
    }
}
