using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Serializable]
    public class EnemyData
    {
        public EnemyState enemyState;
        public Vector3 ePos;
        public Quaternion eRotate;
        public float remainHP;
        public float remainMP;
        public bool patrol;
        public int patrolPoint;
        
        public EnemyData(EnemyState _eState, Vector3 _ePos, Quaternion _eRotate ,float _reHp, float _reMp = 0,bool _patrol = false,int _patNum =0)
        {
            enemyState = _eState;
            ePos = _ePos;
            eRotate = _eRotate;
            remainHP = _reHp;
            remainMP = _reMp;
            if (_patrol)
            {
                patrol = _patrol;
                patrolPoint = _patNum;
            }
        }
    }

    public EnemyData eData;

    public EnemyState eState = EnemyState.Idle;

    //Enemy의 활동여부
    public bool isActive;

    //Enemy의 데이터터 세팅 여부
    public bool isSet = false;

    //ememy 애니메이션
    [HideInInspector]public Animator enemyAnime;

    //시작 체력
    public float startHealth;
    //현재 체력
    public float currentHealth;
    //최대 체력
    protected float maxHealth;

    //처지시 획득 가능한 경험치량
    public float exp;

    //공격
    //공격 범위
    public float attackRange = 2.5f;
    //공격력
    public float attackDamage = 15f;
    //공격 쿨타임
    public float attackCoolTime = 1.5f;
    public float attackTimer = 0;
    public bool isAttack = false;

    //Enemy 회전속도
    public float rotateSpeed = 7;

    //대상(플레이어)
    public GameObject target;
    //플레이어 위치 방향
    protected Vector3 targetDir;
    //플레어와의 거리
    protected float targetDis;

    //Enemy의 시작 위치
    protected Vector3 startPos;
    //시작 위치와의 거리(제자리로 돌아갈때 사용)
    protected float startDis;

    protected NavMeshAgent agent;

    //플레이어 감지 범위 : 감지범위 내에 들어온 플레어를 즉시 추격한다.
    public float detectRange = 15;

    //반응범위 : 해당 범위 내에서 플레이어에게 공격을 받을 시 플레이어를 추격한다.
    public float reactionRange = 30;

    protected float disRange;

    //패트롤 하는 이너미 판정
    public bool isPatroll;

    //패트롤 포인트 리스트
    public List<Transform> patrollpoints;
    //현재 지정된 패트롤 포인트
    protected int patrollNum;
  
    //이너미의 상태 체크
    //죽음 상태
    public bool isDeath = false;

    //캐스팅 상태
    public bool isCasting = false;

    //원래 위치 인가요?
    public bool isReturnhome = false;
    
    //추적하는 상태
    public bool isChasemode = false;
    public float chaseResetCount;
    public float chaseResetTime = 5;

    //플레이어가 보이나요?
    public bool isSeePlayer;

    //도망가는 상태
    public bool isRunaway = false;
    //도망갈 거리
    public float runDis = 5f;
    protected float runDisCount;
    protected Vector3 damageDir;

    //Enemy가 죽을 시 Off될 히트박스
    public CapsuleCollider hitBox;

    //Enemy의 Renderer 모음집
    public GameObject RenderBox;
    //private Vector3 asd = new Vector3(44.5f, 0.5f, -324.75f);

    public GameObject EnemeyMark;

    public Vector3 offset;

    public bool isStop;

    public Sound[] enemySound;
    public string attackSound;
    public string moveSound;
    public string damagedSound;
    public string deadSound;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        /*
        enemyAnime = GetComponentInChildren<Animator>();
        //시작 체력에 현재 체력을 넣어준다.
        currentHealth = startHealth;
        //대부분의 상황에서 시작 체력은 최대 체력이다.
        maxHealth = startHealth;

        agent = GetComponent<NavMeshAgent>();

        //게임 실행시 이너미가 처음 있던 위치, 경우에 따라 직접 설정가능
        startPos = transform.position;

        disRange = reactionRange * 1.5f;

        //시작 시 패트롤 이너미이면
        if (isPatroll)
        {
            //패트롤 위치로 이동
            SetState(EnemyState.Walk);
            agent.SetDestination(patrollpoints[patrollNum].position);
        }
        //아니라면 제자리에 서 있기
        else
        {
            SetState(EnemyState.Idle);
        }

        eData = new EnemyData(eState, transform.position, currentHealth);
        */
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }

        //이너미가 죽거나 활동중이 아닐때 동작하지 않는다.
        if (isDeath || !isActive || isStop || !isSet)
        {
            return;
        }

        PlayerDeath();

        //이너미가 캐스팅 동작 중일때 다른 동작을 하지 않는다.
        if (isCasting)
        {
            //agent.ResetPath();
            return;
        }
        
        //이너미가 플레이어의 위치를 감지한다.
        CheckTargetPos();

        //공격 쿨타임이 회복되면 더 이상 계산하지 않는다.
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (isSeePlayer)
        {
            if (attackRange == detectRange && attackRange == reactionRange)
            {
                NotcognizeRange();
            }
            //Debug.Log("보임");
            else if (targetDis <= attackRange)
            {
                //Debug.Log("공격범위");
                AttackRange();
            }
            //플레이어가 공격범위 밖으로 나가고 감지 범위 내에 있을때
            else if (targetDis > attackRange && targetDis <= detectRange)
            {
                //Debug.Log("감지범위");
                DetectRange();
            }
            //플레이어가 감지범위 밖에 있지만 액션범위 내에 있을 때
            else if (targetDis > detectRange && targetDis <= reactionRange)
            {
                //Debug.Log("액션범위");
                ActionRange();
            }
            //플레이어가 액션범위 밖에 있을때
            else if (targetDis > reactionRange)
            {
                //Debug.Log("무방비범위");
                NotcognizeRange();
            }
        }
        else
        {
            //Debug.Log("안보임");
            if (isChasemode)
            {
                if (targetDis <= detectRange)
                {
                    //Debug.Log("감지범위");
                    DetectRange();
                }
                //플레이어가 감지범위 밖에 있지만 액션범위 내에 있을 때
                else if (targetDis > detectRange && targetDis <= reactionRange)
                {
                    //Debug.Log("액션범위");
                    ActionRange();
                }
                //플레이어가 액션범위 밖에 있을때
                else if (targetDis > reactionRange)
                {
                    //Debug.Log("무방비범위");
                    NotcognizeRange();
                }
            }
            else
            {
                NotcognizeRange();
            }
        }

        SwitchState();
    }

    public virtual void SwitchState()
    {
        switch (eState)
        {
            case EnemyState.Idle:
                //agent.velocity = Vector3.zero;
                break;

            case EnemyState.Walk:
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((startPos - transform.position)), Time.deltaTime * rotateSpeed);
                if (isRunaway)
                {
                    eState = EnemyState.Runaway;
                }
                else
                {
                    if (isPatroll)
                    {
                        //LookRate((patrollpoints[patrollNum].position - transform.position));
                    }
                    else
                    {
                        //LookRate(startPos - transform.position);
                    }
                }
                break;

            case EnemyState.Attack:
                //agent.ResetPath();

                LookChase();
                break;

            case EnemyState.Damaged:

                break;

            case EnemyState.Casting:
                //agent.ResetPath();

                break;

            case EnemyState.Chase:
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPos), Time.deltaTime * rotateSpeed);
                //LookRate(targetDir);              

                agent.SetDestination(target.transform.position);

                /*
                if (distance > detectRange)
                {
                    SetNextPos();
                }
                */
                break;

            case EnemyState.Runaway:
               

                runDisCount -= Time.deltaTime;
                if (runDisCount > 0)
                {
                    LookRate(damageDir);
                    transform.Translate(Vector3.forward * 3.5f * Time.deltaTime);
                }
                else
                {
                    SetState(EnemyState.Idle);
                    Invoke("SetNextPos", 3f);
                }
                /*
                if(agent.remainingDistance < 1f && isRunaway)
                {
                    //도망갈 위치에 도착시 멈춰있다가 일정시간 뒤에 다음위치로 돌아간다.
                    Debug.Log("집에가양");
                    SetState(EnemyState.Idle);
                    //isRunaway = false;
                    Invoke("SetNextPos", 3f);
                }
                */
                break;

            case EnemyState.Death:
                
                break;
        }
    }

    public virtual void AttackRange()
    {
        //플레이어가 공격범위 안에 있고 공격이 가능할 때
        if (attackTimer <= 0)
        {
            //공격
            isAttack = true;
            SetState(EnemyState.Attack);

            //공격에 쿨타임 넣기
            attackTimer = attackCoolTime;
        }
        //플레이어가 공격범위 안에 있고 공격이 불가능 할때
        else if (attackTimer > 0 && !isAttack)
        {
            //플레이어를 바라본다.
            SetState(EnemyState.Idle);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPos), Time.deltaTime * rotateSpeed);
            LookRate(targetDir);
        }
    }

    public virtual void DetectRange()
    {
        //플레이어를 쫓는다.
        SetState(EnemyState.Chase);
    }

    public virtual void ActionRange()
    {

        //피격시 플레이어를 쫓아간다
        //도망상태 일지라도 액션 범위내에서 다시 피격을 당하면 즉시 추적하도록 한다.
        if (isChasemode)
        {
            SetState(EnemyState.Chase);
            chaseResetCount = chaseResetTime;
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
        }
    }

    public virtual void NotcognizeRange()
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
            }
        }
        else
        {
            SetNextPos();
        }
    }

    public virtual void ResetState()
    {
        isCasting = false;
        isAttack = false;
        isChasemode = false;
    }

    public virtual void SetState(EnemyState _state)
    {
        //같은 상태를 받을 시 애니메이션 끊기지 않게 리턴해준다.
        if (eState == _state || isDeath)
        {
            return;
        }

        eState = _state;
        //Debug.Log(eState);

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
    
    public void CheckTargetPos()
    {
        if (reactionRange >= (target.transform.position - transform.position).magnitude)
        {
            //targetDir = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position;
            targetDir = (target.transform.position - transform.position).normalized;
            targetDis = (target.transform.position - transform.position).magnitude;
            SeeThePlayer();
        }
        else if(reactionRange < (target.transform.position - transform.position).magnitude && disRange > (target.transform.position - transform.position).magnitude)
        {
            isSeePlayer = false;
            //targetDis = (target.transform.position - transform.position).magnitude;
            targetDis = disRange;
        }
    }

    protected virtual void SetNextPos()
    {
        isRunaway = false;
        if (isPatroll)
        {
            Vector3 myPos = transform.position;
            if ((patrollpoints[patrollNum].position - transform.position).magnitude < 1f)
            {             
                if (patrollNum == patrollpoints.Count-1)
                {
                    patrollNum = 0;
                }
                else
                {
                    patrollNum++;
                }
                //Debug.Log(patrollNum);
                SetState(EnemyState.Walk);
                agent.SetDestination(patrollpoints[patrollNum].position);
            }
            SetState(EnemyState.Walk);
            agent.SetDestination(patrollpoints[patrollNum].position);
        }
        else
        {
            if ((startPos - transform.position).magnitude >= 1.0f)
            {
                isReturnhome = false;              
                SetState(EnemyState.Walk);
                agent.SetDestination(startPos);
                
            }
            else if ((startPos - transform.position).magnitude < 1.0f && !isReturnhome)
            {
                isReturnhome = true;
                SetState(EnemyState.Idle);
                
            }
        }
    }

    public virtual void LookChase() 
    {
        LookRate(targetDir);
    }

    public void LookRate(Vector3 _rotateDir)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_rotateDir), Time.deltaTime * rotateSpeed);
    }

    public virtual void TakeDamage(float _damage)
    {
        if (isDeath)
        {
            return;
        }
        if (_damage > 0)
        {
            currentHealth -= _damage;
            Dagamed();
            Debug.Log(currentHealth/maxHealth*100 + " %");
            PlayEnemySound(damagedSound);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //이너미와 플레이어의 거리에 따라 이너미가 피격시 취하는 행동
    public void Runaway(Vector3 _damageDir)//공격 받은 방향
    {
        damageDir = _damageDir;

        //현재 이너미 위치로부터 damageDir방향으로 runDis 떨어진 위치
        Vector3 runpos = transform.position + (damageDir.normalized * runDis);
        runpos.y = 0.5f;
        //Debug.Log(transform.name + " : " + runpos);

        //플레이어와의 거리가 액션범위 밖일때 (무방비 상태 일 때)
        if ((transform.position - target.transform.position).magnitude > reactionRange)
        {
            //무방비 상태일 때 피격시 runpos 위치로 도망간다.
            isRunaway = true;
            SetState(EnemyState.Runaway);
            runDisCount = runDis;
            //agent.SetDestination(runpos);
            //agent.SetDestination(new Vector3(-37.03f, 0.5f, 42.90f));          
        }
    }

    protected void PlayerDeath()
    {
        if (target.GetComponent<PlayerStatus>().isDeath && !isStop)
        {
            isStop = true;
            ResetState();
            if (eState != EnemyState.Death)
            {
                SetState(EnemyState.Idle);
            }
        }
    }

    public virtual void Dagamed()
    {
        //플레이어가 액션 범위(공격범위,감지범위 포함) 안에서 공격 받을 시 추적모드로 바꿔준다.
        if ((transform.position-target.transform.position).magnitude <= reactionRange)
        {
            //Debug.Log(targetDis);
            isChasemode = true;
            isRunaway = false;
            chaseResetCount = chaseResetTime;
        }
    }

    public void SeeThePlayer()
    {
        Debug.DrawRay(transform.position+offset, targetDir * targetDis, Color.black);
        //RaycastHit[] hit = Physics.RaycastAll(transform.position, targetDir, targetDis, 4160);
        //플레이어 방향으로 레이를 쏴서 벽에 닿으면
        if (Physics.Raycast(transform.position+ offset, targetDir, targetDis, 1<<12))
        {
            //시야가 벽에 막혔기 때문에 플레이어가 안보인다고 판정한다.
            isSeePlayer = false;
        }
        else
        {
            //시야가 벽에 막히기 않았기 때문에 플레이어가 보인다고 판정한다.
            isSeePlayer = true;
        }
    }
    
    public virtual void Die()
    {
        isDeath = true;
        PlayEnemySound(deadSound);
        eState = EnemyState.Death;
        enemyAnime.SetBool("isDeath", isDeath);
        target.GetComponent<PlayerStatus>().AddExp(exp);
        EnemeyMark.SetActive(false);
        hitBox.enabled = false;
    }

    public void PlayEnemySound(string _eSound)
    {
        Sound sound = null;
        foreach (var s in enemySound)
        {
            if (s.name == _eSound)
            {
                sound = s;
                break;
            }
        }

        if(_eSound == "olf_telpoout")
        {
            Debug.Log(sound.clip.length);
        }

        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.Log(_eSound);
        }
    }

    public void StopEnemySound(string _eSound)
    {
        Sound sound = null;
        foreach (var s in enemySound)
        {
            if (s.name == _eSound)
            {
                sound = s;
                break;
            }
        }

        if (sound != null)
        {
            sound.source.Stop();
        }
        else
        {
            Debug.Log(_eSound);
        }
    }

    public virtual void SetData()
    {
        //Debug.Log("세팅");
        enemyAnime = GetComponentInChildren<Animator>();

        //사운드 세팅
        AddSound();

        //시작 체력에 현재 체력을 넣어준다.
        currentHealth = startHealth;
        //대부분의 상황에서 시작 체력은 최대 체력이다.
        maxHealth = startHealth;

        agent = GetComponent<NavMeshAgent>();

        //게임 실행시 이너미가 처음 있던 위치, 경우에 따라 직접 설정가능
        startPos = transform.position;

        disRange = reactionRange * 1.5f;
        //시작 시 패트롤 이너미이면
        if (isPatroll)
        {
            //패트롤 위치로 이동
            SetState(EnemyState.Walk);
            agent.SetDestination(patrollpoints[patrollNum].position);
        }
        //아니라면 제자리에 서 있기
        else
        {
            SetState(EnemyState.Idle);
        }

        eData = new EnemyData(eState, transform.position, transform.rotation, currentHealth);
    }

    public virtual EnemyData SaveState()
    {
        eData.enemyState = eState;
        eData.ePos = transform.position;
        eData.eRotate = transform.rotation;
        eData.remainHP = currentHealth;
        if (isPatroll)
        {
            eData.patrol = true;
            eData.patrolPoint = patrollNum;
        }
        else
        {
            eData.patrol = false;
        }

        return eData;           
    }

    public virtual void LoadState(EnemyData _eData)
    {
        //Debug.Log("eDataLoad");
        eData = _eData;
        //eState = eData.enemyState;
        SetState(eData.enemyState);
        transform.position = eData.ePos;
        transform.rotation = eData.eRotate;

        if (eData.enemyState == EnemyState.Death)
        {
            isDeath = true;
            currentHealth = 0;
            hitBox.enabled = false;
            enemyAnime.SetBool("isDeath", isDeath);
            EnemeyMark.SetActive(false);
        }
        else
        {
            currentHealth = eData.remainHP;
            if (eData.patrol)
            {
                isPatroll = true;
                patrollNum = eData.patrolPoint;
            }
        }
    }

    public void AddSound()
    {
        foreach (var s in enemySound)
        {
            //AudioManager.instance.AddExternalSound(s);
            s.source = gameObject.AddComponent<AudioSource>();
            //Sound 클래스를 통해 만든 사운드 정보를 가져와 AudioSource에 세팅해 준다.
            //Sound 클래스에는 재생할 오디오 clip과 
            //그 클립에 접근할 오디오 이름, 처음 설정될 volume과 pitch값과 반복 여부를 가져와 세팅해준다.
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.spatialBlend = 1;
            s.source.rolloffMode = AudioRolloffMode.Custom;
            s.source.maxDistance = 50;

            //사운드마다 사용할 용도가 다르기 때문에 용도에 따라 구분하여 믹서 그룹에 저장해 준다.

            //bgm은 bgm 믹서에 저장
            if (s.soundType == SoundType.BGM)
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[1];
            }
            //효과음은 sfx에 저장
            else if (s.soundType == SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[2];
            }
            //나머지(현재는 환경음) Ambience에 저장
            else
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[3];
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Obstacle") && isRunaway)
        {
            //Relect(입사각(transform.forward), 법선벡터(collision.GetContact(0).normal)) = 반사각
            damageDir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
        }
    }
}

public enum EnemyState
{
    Idle,
    Walk,
    Attack,
    Damaged,
    Casting,
    Chase = 11,
    Runaway = 12,
    Death = 100,
}
