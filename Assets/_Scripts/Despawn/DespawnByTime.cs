using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnByTime : Despawn
{
    [SerializeField] protected float delay = 2f;
    [SerializeField] protected float timer = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected virtual void ResetTimer()
    {
        this.timer = 0;
    }

    protected override bool CanDespawn()
    {
        this.timer += Time.deltaTime;
        if (this.timer > this.delay) return true;
        return false;
    }
}
