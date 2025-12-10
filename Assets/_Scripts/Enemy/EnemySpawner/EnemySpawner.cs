using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Spawner
{
    private static EnemySpawner instance;
    public static EnemySpawner Instance => instance;

    public static string enemyOne = "Slime";

    protected override void Awake()
    {
        base.Awake();
        if (EnemySpawner.instance != null) Debug.LogError("More than one instance of Enemy Spawner found!");
        EnemySpawner.instance = this;
    }
}
