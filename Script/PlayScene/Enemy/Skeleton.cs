using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Enemy
{
    public bool spawnEnemy = false;
    public BoxCollider sword;

    private List<Collider> player = new List<Collider>();
    protected override void Start()
    {
        base.Start();

        if (spawnEnemy)
        {
            SetData();
            isActive = false;
            target = GameObject.FindWithTag("Player");
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            Invoke("AwakeEnemy", 2f);
        }      
    }

    public override void AttackRange()
    {
        if (attackTimer <= 0)
        {
            isAttack = true;
            sword.enabled = true;
            SetState(EnemyState.Attack);
            attackTimer = attackCoolTime;
        }
        else if (attackTimer > 0 && !isAttack)
        {
            SetState(EnemyState.Idle);
            LookRate(targetDir);
        }
    }

    public void EndAttack()
    {
        isAttack = false;
        sword.enabled = false;
        SetState(EnemyState.Idle);
        player.Clear();
    }

    /*
    public override void Die()
    {      
        
        isDeath = true;
        enemyAnime.SetBool("isDeath", isDeath);
        hitBox.enabled = false;
        agent.enabled = false;
        sword.enabled = false;
        target.GetComponent<PlayerStatus>().AddExp(exp);
    }
    */

    public override void Die()
    {
        base.Die();      
        agent.enabled = false;
        sword.enabled = false;
    }

    public override void LoadState(EnemyData _eData)
    {
        base.LoadState(_eData);
        if (eData.enemyState == EnemyState.Death)
        {
            agent.enabled = false;
            sword.enabled = false;
        }
    }
    
    private void AwakeEnemy()
    {
        isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player"))
        {
            if (!player.Contains(other))
            {
                player.Add(other);               
                other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);
                //Debug.Log(this.name);
            }
        }
    }
}

