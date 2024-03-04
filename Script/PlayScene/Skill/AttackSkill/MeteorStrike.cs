using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorStrike : SkillManager
{
    //public bool isCasting;
    //캐스팅시 보여지는 오라
    public ParticleSystem castAuraPrefab;
    public GameObject meteorPrefab;
    //스킬이 시전될 위치를 표시해주는 마법진
    public ParticleSystem meteorCirclePrefab;

    //스킬 공격력
    public float skillDamage;

    //캐스팅 모션 블랜드 트리안에서 선택한 모션트리 번호
    private float castMotionSelect;

    //현재 생성된 캐스팅 오라
    private ParticleSystem castAura;

    //현재 생성된 스킬 마법진
    private ParticleSystem meteorCIrcle;
    //마우스 위치에 따라 변경될 스킬 마법진 좌표
    private Vector3 circlePos;

    public AudioSource castingSound;
    public AudioSource startCasting;


    private void Update()
    {
        if (!isUse)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                isUse = true;
            }
        }

        //스킬이 사용되어 캐스팅 중일 때
        if (PlayerController.isCasting)
        {
            RaycastHit hit;
            Vector3 hitpos;

            //마우스위치에서 레이캐스트를 쏴서 마법진 이펙트를 표시시켜준다.
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1))
            {
                hitpos = hit.point;
                if (meteorCIrcle != null)
                {
                    circlePos = hitpos;
                    circlePos.y += 0.5f;
                    meteorCIrcle.transform.position = circlePos;
                }
                //마우스 좌클릭시
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit _hit;
                    Vector3 _hitpos;

                    //해당 위치의 오브젝트를 판별하여
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit, Mathf.Infinity))
                    {
                        //히트된 오브젝트가 water, obstacle , object가 아니고
                        if (/*!_hit.transform.CompareTag("Water") &&*/ !_hit.transform.CompareTag("Obstacle") && !_hit.transform.CompareTag("Object"))
                        {
                            //시야가 확보된 공간이면
                            _hitpos = _hit.point;
                            if (player.GetComponentInChildren<PlayerSight>().Dropable(hitpos))
                            {
                                //메테오 시전
                                {
                                    castingSound.loop = false;

                                    PlayerController.isCasting = false;
                                    //블랜드된 캐스팅 모션 실행
                                    player.GetComponentInChildren<Animator>().SetFloat("MotionProccess", 1);
                                    //블랜드 중이기 때문에 모션을 처음부터 실행하여 자연스러운 연출 부여
                                    player.GetComponentInChildren<Animator>().Play("Player_Cast_Tree", -1, 0);

                                    //블랜드 트리를 사용하지 않았을 때 애니메이션 구간 반복을 넘어가기 위해
                                    //addEvent가 등록된 구간 보다 앞구간(0.41f)을 실행 시켜준다.
                                    //player.GetComponentInChildren<Animator>().Play("Footman_Action", -1, 0.41f);
                                    Cursor.visible = true;
                                    Destroy(meteorCIrcle.gameObject);
                                    Destroy(castAura.gameObject, 2f);

                                    GameObject _meteor = Instantiate(meteorPrefab, new Vector3(hitpos.x, 0, hitpos.z), Quaternion.identity);
                                    _meteor.GetComponent<Meteor>().SetMeor(new Vector3(hitpos.x, 0, hitpos.z));
                                    float _skillDamage = player.playerStatus.currentDamage * skillDamage;
                                    _meteor.GetComponent<Meteor>().SetDagage(_skillDamage);
                                }
                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) /*| Input.GetKeyDown(KeyCode.Escape)*/)
            {
                PlayerController.isCasting = false;
                player.SetState(PlayerState.Idle);
                player.GetComponentInChildren<Animator>().SetFloat("MotionProccess", 0);

                Cursor.visible = true;
                Destroy(meteorCIrcle.gameObject);
                Destroy(castAura.gameObject);

                castingSound.loop = false;
            }
        }
    }

    public void LateUpdate()
    {
        if (PlayerController.isCasting)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PlayerController.isCasting = false;
                player.SetState(PlayerState.Idle);
                player.GetComponentInChildren<Animator>().SetFloat("MotionProccess", 0);

                Cursor.visible = true;
                Destroy(meteorCIrcle.gameObject);
                Destroy(castAura.gameObject);

                castingSound.loop = false;
            }
        }
    }


    public override void UseSkill(PlayerController _player)
    {
        if (isUse)
        {
            player = _player;
            player.GetComponent<PlayerStatus>().UseMana(cunsumeMana);
            player.GetComponentInChildren<Weapon>().SetSkillDage(skillDamage,true);
            player.SetState(PlayerState.Casting);
            player.SetCastMotion(castMotionSelect);
            /*
            player.SetState(PlayerState.Action);
            player.SetAnime(skillMotion);
            player.SetActionSpeed(actionSpeed);
            */

            PlayerController.isAction = true;

            isUse = false;
            PlayerController.isCasting = true;
            remainingTime = coolTime;

            Cursor.visible = false;

            startCasting.Play();
            castingSound.Play();
            castingSound.loop = true;

            //스킬 마법진 생성
            ParticleSystem _meteorCircle = Instantiate(meteorCirclePrefab, _player.effectPos);          
            meteorCIrcle = _meteorCircle;

            ParticleSystem _castAura = Instantiate(castAuraPrefab, new Vector3(_player.transform.position.x,0.5f,_player.transform.position.z), Quaternion.identity);
            castAura = _castAura;
        }
    }
}


