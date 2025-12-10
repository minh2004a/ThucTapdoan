using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPoints : GameMonoBehaviour
{
    [SerializeField] protected List<Transform> points;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadPoints();
    }

    protected virtual void LoadPoints()
    {
        if (this.points.Count > 0) return;
        foreach(Transform Points in transform)
        {
            this.points.Add(Points);
        }
    }

    public virtual Transform GetRandom()
    {
        int rand = Random.Range(0, this.points.Count);
        return points[rand];
    }
}
