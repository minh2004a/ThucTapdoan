using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : GameMonoBehaviour
{
    [SerializeField] protected Transform model;
    public Transform Model => model;

    [SerializeField] protected EnemyDespawn enemyDespawn;
    public EnemyDespawn EnemyDespawn => enemyDespawn;

    [SerializeField] protected EnemySO enemySO;
    public EnemySO EnemySO => enemySO;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadModel();
        this.LoadEnemySO();
        this.LoadEnemyDespawn();
    }

    protected virtual void LoadModel()
    {
        if (this.model != null) return;
        this.model = transform.Find("Model");
        Debug.LogWarning(transform.name + ": LoadModel", gameObject);
    }

    protected virtual void LoadEnemySO()
    {
        if (this.enemySO != null) return;
        string resPath = "Enemy/" + transform.name;
        this.enemySO = Resources.Load<EnemySO>(resPath);
        Debug.LogWarning(transform.name + ": LoadEnemySO " + resPath, gameObject);
    }
    protected virtual void LoadEnemyDespawn()
    {
        if (this.enemyDespawn != null) return;
        this.enemyDespawn = transform.GetComponentInChildren<EnemyDespawn>();
        Debug.LogWarning(transform.name + ": LoadEnemyDespawn", gameObject);
    }
}
