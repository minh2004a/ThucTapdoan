using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageSender : MonoBehaviour
{
    [SerializeField] private EnemyCtrl enemyCtrl;

    [Header("Hiting")]
    [SerializeField] private float hitCooldown = 0.8f;
    private float lastHitTime = -999f;

    private void Awake()
    {
        if (enemyCtrl == null)
        enemyCtrl = GetComponentInParent<EnemyCtrl>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        TryHit(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time >= lastHitTime + hitCooldown)
        TryHit(collision);
    }

    private void TryHit(Collider2D collider)
    {
        PlayerHealth player = collider.GetComponent<PlayerHealth>();
        if (player != null)
        {
            int damage = enemyCtrl.EnemySO.damage;
            player.TakeDamage(damage);

            lastHitTime = Time.time;
        }
    }
}
