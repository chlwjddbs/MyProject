using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElemental : Enemy
{
    public bool isPase_2 = false;
    public GameObject FireAura;
    public GameObject pase2_Effect;
    public GameObject DeathEffect;
    public FireElementalTrigger fireElementalTrigger;

    public  void NextPase()
    {
        if(currentHealth <= maxHealth / 2 && !isPase_2)
        {
            PlayEnemySound("page2");
            StartCoroutine("ChangePase");
        }
    }

    IEnumerator ChangePase()
    {
        isCasting = true;
        pase2_Effect.SetActive(true);
        GetComponent<FireElementalAction>().RotateBurn();
        SetState(EnemyState.Casting);
        isPase_2 = true;
        attackCoolTime = attackCoolTime - (attackCoolTime / 3f);

        yield return new WaitForSeconds(4f);
        isCasting = false;
        SetState(EnemyState.Idle);
    }

    public override void TakeDamage(float _damage)
    {
        if (isDeath)
        {
            return;
        }
        if (_damage > 0)
        {
            currentHealth -= _damage;
            Dagamed();
            Debug.Log(currentHealth / maxHealth * 100 + " %");
            PlayEnemySound(damagedSound);
        }

        NextPase();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        StartCoroutine("FireElementalDie");
        target.GetComponent<PlayerStatus>().AddExp(exp);
    }

    IEnumerator FireElementalDie()
    {
        isDeath = true;
        eState = EnemyState.Death;
        FireAura.GetComponent<ParticleSystem>().Stop();
        enemyAnime.SetBool("isDeath", isDeath);
        AudioManager.instance.StopBGM(1f);
        PlayEnemySound(deadSound);

        yield return new WaitForSeconds(0.3f);

        Instantiate(DeathEffect, transform.position, Quaternion.identity);
        PlayEnemySound("gn_explosion");

        yield return new WaitForSeconds((enemySound[3].clip.length)-0.3f);

        gameObject.SetActive(false);
        AudioManager.instance.PlayBGM("PlayScene_Floor_1",1f);
    }

    public override void LoadState(EnemyData _eData)
    {
        eData = _eData;
        eState = eData.enemyState;
        if (eData.enemyState == EnemyState.Death)
        {
            isDeath = true;
            currentHealth = 0;
            FireAura.GetComponent<ParticleSystem>().Stop();
            enemyAnime.SetBool("isDeath", isDeath);
            gameObject.SetActive(false);
            Destroy(fireElementalTrigger.cursorAction.gameObject);
        }
    }
}
