using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lich : Enemy
{
    public GameObject lichballPrefab;
    public Transform lichballPoint;

    public GameObject spawnskeleonsPrefab;
    public GameObject spawnskeleonsAuraPrefab;
    public Transform spawnPoint;
    public float spawnskeleonsCooltime = 100f;
    public float coolDown = 0f;
    private bool isSkill = false;
    private bool isMove = false;

    public GameObject lichAura;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        SpawnSkeleton();
    }

    public override void Dagamed()
    {
        if (targetDis <= reactionRange)
        {
            isChasemode = true;
            enemyAnime.SetBool("isChaseMode", true);           
            isRunaway = false;
            chaseResetCount = chaseResetTime;
        }
    }
    public override void DetectRange()
    {
        SetState(EnemyState.Chase);
        enemyAnime.SetBool("isChaseMode", true);
    }

    public override void ActionRange()
    {

        //피격시 플레이어를 쫓아간다
        //도망상태 일지라도 액션 범위내에서 다시 피격을 당하면 즉시 추적하도록 한다.
        if (isChasemode)
        {
            SetState(EnemyState.Chase);
        }
        else if (isRunaway)
        {
            //무방비 상태에서 피격받아 도망 상태가 된 후
            //플레이어가 액션 범위내로 들어오면 공격하지 않아도 계속 도망가야한다.
            //그렇기 때문에 도망상태 일때 계속 도망 갈 수 있도록 아무런 판정을 주지 않는다.
        }
        else
        {
            SetNextPos();
            enemyAnime.SetBool("isChaseMode", false);
        }
    }
    public override void NotcognizeRange()
    {
        if (isRunaway)
        {
        }
        else if (isChasemode)
        {
            chaseResetCount -= Time.deltaTime;
            if (chaseResetCount <= 0)
            {
                isChasemode = false;
                SetNextPos();
                enemyAnime.SetBool("isChaseMode", false);
            }
        }
        else
        {        
            SetNextPos();
            enemyAnime.SetBool("isChaseMode", false);
        }
    }

    public void Attack()
    {
        GameObject Lichball = Instantiate(lichballPrefab,lichballPoint);
        Lichball.GetComponent<LichBall>().GetTargetPos(target.transform.position,target,attackDamage);
        isSkill = true;
    }
    public override void Die()
    {
        base.Die();
        //isDeath = true;
        //enemyAnime.SetBool("isDeath", isDeath);
        hitBox.enabled = false;
        agent.enabled = false;
        lichAura.SetActive(false);
        //target.GetComponent<PlayerStatus>().AddExp(exp);
    }

    public void SpawnSkeleton()
    {
        if (isDeath | !isSkill)
        {
            return;
        }

        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
        else
        {
            PlayEnemySound("spawnSkeleton");
            Instantiate(spawnskeleonsPrefab, spawnPoint.position, Quaternion.identity);
            GameObject spawnskeleonsAura = Instantiate(spawnskeleonsAuraPrefab, spawnPoint.position, Quaternion.identity);
            coolDown = spawnskeleonsCooltime;
            Destroy(spawnskeleonsAura, 2f);
        }
    }

    public override void SetState(EnemyState _state)
    {
        //같은 상태를 받을 시 애니메이션 끊기지 않게 리턴해준다.
        if (eState == _state || isDeath)
        {
            return;
        }

        eState = _state;

        //Debug.Log(eState);
        if(eState == EnemyState.Walk || eState == EnemyState.Chase || eState == EnemyState.Runaway)
        {
            if (!isMove)
            {
                isMove = true;
                PlayEnemySound(moveSound);
            }
        }
        else
        {
            isMove = false;
            StopEnemySound(moveSound);
        }

        if (_state == EnemyState.Chase || _state == EnemyState.Runaway)
        {
            enemyAnime.SetInteger("eState", (int)(EnemyState.Walk));
        }
        else
        {         
            enemyAnime.SetInteger("eState", (int)eState);
        }

        //상태가 바뀔 때 패스를 리셋해 준다.
        //공격하거나 죽거나 등 멈추는 행동을 할 때 경로가 남아있으면 미끄러지는듯한 애니메이션을 방지함
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
}
