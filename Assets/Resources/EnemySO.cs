using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "SO/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName = "New Enemy";
    public int maxHp = 100;

    public int damage = 5;
    // public List<DropRate> dropRates;
}
