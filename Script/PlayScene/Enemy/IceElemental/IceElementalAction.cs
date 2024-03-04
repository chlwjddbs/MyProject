using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceElementalAction : MonoBehaviour
{
    //테일즈위버 Dive 브금
    public IceElemental iceElemental;
    //몬스터의 공격 히트박스
    public CapsuleCollider AttackPoint;

    //몬스터의 패시브 스킬(icecleBoom)
    //스킬 생성 위치
    private Vector3 icicleBoomPos;
    //스킬 이펙트 프리팹
    public GameObject iceBoomPrefab;
    //스킬이 생성될 범위
    public float iceBoomRange;

    //icicleBoom 시전시 사전 표시 마법진
    public GameObject icicleCirclePrefab;

    public float iceBlastGauge;
    private float MaxGauge = 100;
    [HideInInspector] public bool isIceBlast = false;

    public GameObject iceBlastCirclePrefab;
    private Vector3 iceBlastCirclePos = new Vector3(-131.75f, 0.6f, -175f);

    private Vector3 iceElementalOrignPos;

    public GameObject teleport;

    private bool isDeath = false;

    public Sound iceBoomSound;

    public GameObject iceElementalBody;

    private void Start()
    {
        //icicleBoom을 몬스터가 활성화 된 1초 후 2초마다 반복 실행
        InvokeRepeating("TripleIcicleBoom", 1f ,2.5f);
        iceElementalOrignPos = transform.position;
        AudioManager.instance.AddExternalSound(iceBoomSound);
    }

    private void Update()
    {
        //몬스터가 즉으면
        if (iceElemental.isDeath && !isDeath)
        {
            isDeath = true;
            iceElemental.isAttack = false;
            //공격도중 죽으면 히트박스가 남기 떄문에 히트박스 오프
            AttackPoint.enabled = false;
            //icicleBoom 오프
            CancelInvoke("TripleIcicleBoom");
            StopCoroutine("Blast");
        }

        
    }

    public void StartAttack()
    {
        //공격시 히트박스 온
        AttackPoint.enabled = true;
    }

    public void EndAttack()
    {
        iceElemental.isAttack = false;
        iceElemental.eState = EnemyState.Idle;
        iceElemental.enemyAnime.SetInteger("eState", (int)iceElemental.eState);
        AttackPoint.enabled = false;
    }

    IEnumerator IcicleBoom()
    {
        //icicleBoom을 몬스터를 기준으로 iceBoomRange거리 내에 랜덤으로 뿌려주는 좌표 얻기
        icicleBoomPos.x = iceElemental.GetComponent<Transform>().position.x + (Random.insideUnitCircle.normalized.x * iceBoomRange);
        icicleBoomPos.y = 1.5f;
        icicleBoomPos.z = iceElemental.GetComponent<Transform>().position.z + (Random.insideUnitCircle.normalized.y * iceBoomRange);

        Vector3 iciclePos = icicleBoomPos;

        GameObject iceCIrcle = Instantiate(icicleCirclePrefab, iciclePos, Quaternion.identity);
        Destroy(iceCIrcle, 1f);

        yield return new WaitForSeconds(1f);
        //icicleBoom 생성 후 2초 뒤 삭제
        GameObject iceBoom = Instantiate(iceBoomPrefab, iciclePos, Quaternion.identity);
        Destroy(iceBoom, 2f);
        
    }

    public void TripleIcicleBoom()
    {
        StartCoroutine("TripleBoom");
    }

    //icicleBoom 3개 생성
    IEnumerator TripleBoom()
    {
        StartCoroutine("IcicleBoom");
        yield return new WaitForSeconds(.1f);

        StartCoroutine("IcicleBoom");
        yield return new WaitForSeconds(.1f);

        StartCoroutine("IcicleBoom");
        yield return new WaitForSeconds(.8f);

        iceElemental.PlayEnemySound("olf_iceboom");
    }

    public void AddBlastGauge(float Point)
    {
        //iceBlast 시전 중엔 게이지가 차지 않게한다.
        if (isIceBlast)
        {
            return;
        }

        //플레이어가 icicleBoom에 맞으면 iceBlast 게이지 획득
        iceBlastGauge += Point;
        iceBlastGauge = Mathf.Clamp(iceBlastGauge, 0, MaxGauge);

        //Blast 게이지가 100이 되면 iceBloast 시전
        if (iceBlastGauge == MaxGauge && isIceBlast == false)
        {
            StartCoroutine("Blast");
        }
    }

    IEnumerator Blast()
    {
        isIceBlast = true;
        iceElemental.isAttack = false;
        Instantiate(teleport, transform.position, Quaternion.identity);
        
        AttackPoint.enabled = false;
        iceElemental.isCasting = true;

        iceElemental.PlayEnemySound("olf_telpoin");

        iceElementalBody.SetActive(false);

        yield return new WaitForSeconds(1.25f);

        iceElemental.PlayEnemySound("olf_telpoout");

        yield return new WaitForSeconds(0.9f);

        transform.position = iceElementalOrignPos;
        iceElementalBody.SetActive(true);
        iceElemental.FollowIceEffect();
        iceElemental.SetState(EnemyState.Casting);
        GameObject iceBlastCircle = Instantiate(iceBlastCirclePrefab, iceBlastCirclePos, Quaternion.identity);
        
        Debug.Log("IceBlast");

        yield return new WaitForSeconds(5f);

        iceBlastCircle.GetComponent<IceBlast>().magicCircle.Stop();
        iceBlastCircle.GetComponent<IceBlast>().light_1.Stop();
        iceBlastCircle.GetComponent<IceBlast>().light_2.Stop();

        yield return new WaitForSeconds(1f);

        iceBlastCircle.GetComponent<IceBlast>().stopSound();
        iceBlastCircle.GetComponent<IceBlast>().iceBlast.SetActive(true);

        yield return new WaitForSeconds(.5f);

        iceBlastCircle.GetComponent<SphereCollider>().enabled = true;

        yield return new WaitForSeconds(1f);

        iceBlastCircle.GetComponent<SphereCollider>().enabled = false;
        iceElemental.isCasting = false;
        iceBlastGauge = 0f;
        iceElemental.SetState(EnemyState.Idle);

        yield return new WaitForSeconds(2f);
        isIceBlast = false;
        Destroy(iceBlastCircle);
    }
}
