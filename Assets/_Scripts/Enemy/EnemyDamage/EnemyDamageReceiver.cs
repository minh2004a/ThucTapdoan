using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageReceiver : MonoBehaviour, IDamageable
{
    [Header("Enemy")]
    public int maxHp = 50;
    public int currentHp;
    [SerializeField] protected EnemyCtrl enemyCtrl;
    [SerializeField] private Animator animator;

    private bool isDead = false;

    private void Awake()
    {
        if (enemyCtrl == null) enemyCtrl = GetComponentInChildren<EnemyCtrl>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        this.Reborn();
    }

    public void TakeHit(int damage)
    {
        if (isDead) return;

        currentHp -= damage;

        DamagePopup.Create(enemyCtrl.transform.position + Vector3.up * 0.5f, damage, DamagePopup.PopupType.PlayerDamage);

        if (currentHp <= 0)
        {
            currentHp = 0;
            this.Die();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
    }

    public void Reborn()
    {
        currentHp = enemyCtrl.EnemySO.maxHp;
        maxHp = currentHp;
        isDead = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
            StartCoroutine(DelayDespawn());
        }
        else
        {
            enemyCtrl.EnemyDespawn.DespawnObject();
        }
    }

    private IEnumerator DelayDespawn()
    {
        float dieLength = 1f;

        foreach(var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Die")
            {
                dieLength = clip.length;
                break;
            }
        }

        yield return new WaitForSeconds(dieLength);

        enemyCtrl.EnemyDespawn.DespawnObject();
    }


}
