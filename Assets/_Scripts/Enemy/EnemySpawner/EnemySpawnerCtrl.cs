using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerCtrl : GameMonoBehaviour
{
    [SerializeField] protected EnemySpawner enemySpawner;
    public EnemySpawner EnemySpawner => enemySpawner;

    [SerializeField] protected EnemySpawnerPoints enemySpawnerPoints;
    public EnemySpawnerPoints EnemySpawnerPoints => enemySpawnerPoints;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadEnemySpawner();
        this.LoadEnemySpawnerPoints();
    }

    protected virtual void LoadEnemySpawner()
    {
        if (this.enemySpawner != null) return;
        this.enemySpawner = this.GetComponent<EnemySpawner>();
        Debug.Log(transform.name + ": Load Enemy Spawner", gameObject);
    }

    protected virtual void LoadEnemySpawnerPoints()
    {
        if (this.enemySpawnerPoints != null) return;
        this.enemySpawnerPoints = this.GetComponentInChildren<EnemySpawnerPoints>();
        Debug.Log(transform.name + ": Load Enemy Spawner Points", gameObject);
    }
}
