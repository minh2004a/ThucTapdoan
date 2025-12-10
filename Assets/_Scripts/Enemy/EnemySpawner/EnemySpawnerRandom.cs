using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerRandom : GameMonoBehaviour
{
    [SerializeField] protected EnemySpawnerCtrl enemySpawnerCtrl;
    [SerializeField] protected float randomDelay = 1f;
    [SerializeField] protected float randomTimer = 0f;
    [SerializeField] protected float randomLimit = 4f;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadEnemySpawnerCtrl();
    }

    protected virtual void LoadEnemySpawnerCtrl()
    {
        if (this.enemySpawnerCtrl != null) return;
        this.enemySpawnerCtrl = this.GetComponent<EnemySpawnerCtrl>();
        Debug.Log(transform.name + ": Load Enemy Spawner Ctrl", gameObject);
    }

    protected virtual void FixedUpdate()
    {
        this.EnemySpawning();
    }

    protected virtual void EnemySpawning()
    {
        if (this.RandomReachLimit()) return;

        this.randomTimer += Time.fixedDeltaTime;
        if (this.randomTimer < this.randomDelay) return;
        this.randomTimer = 0f;

        Transform ranPoint = this.enemySpawnerCtrl.EnemySpawnerPoints.GetRandom();
        Vector3 pos = ranPoint.position;
        Quaternion rot = transform.rotation;

        Transform prefab = this.enemySpawnerCtrl.EnemySpawner.RandomPrefab();
        Transform obj = this.enemySpawnerCtrl.EnemySpawner.Spawn(prefab, pos, rot);
        obj.gameObject.SetActive(true);
    }

    protected virtual bool RandomReachLimit()
    {
        int currentEnemy = this.enemySpawnerCtrl.EnemySpawner.SpawnedCount;
        return currentEnemy >= this.randomLimit;
    }
}
