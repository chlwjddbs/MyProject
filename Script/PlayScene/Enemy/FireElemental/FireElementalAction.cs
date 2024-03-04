using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElementalAction : MonoBehaviour
{
    public FireElemental fireElemental;
    
    public Transform treepleBurn;
    public GameObject burnCirclePrefab;
    public GameObject burnPrefab;

    //public float Test;

    public float burnCount;
    private float startburnStack = 3;
    private float resetCount = 0;

    private void Update()
    {
        //3스텍이 쌓이면
        if (burnCount == startburnStack)
        {
            //Burn을 회전시킨다
            RotateBurn();
        }
    }

    public void TreepleBurn()
    {
        StartCoroutine("SpawnBurn");
    }

    IEnumerator SpawnBurn()
    {
        //공격시 Burn이 소환 될 위치를 미리 알려주는 마법진 생성
        //소환 될 위치 : 플레이어의 위치
        Vector3 burnPoint = new Vector3(fireElemental.target.transform.position.x, 1f, fireElemental.target.transform.position.z);
        GameObject burn_circle = Instantiate(burnCirclePrefab, burnPoint, Quaternion.identity);      

        yield return new WaitForSeconds(1f);

        Vector3 burnpos = burn_circle.transform.position;

        //페이즈 1
        if (!fireElemental.isPase_2)
        {
            GameObject _burn = Instantiate(burnPrefab, burnpos, Quaternion.identity, treepleBurn);
            _burn.GetComponent<RotateAroundBurn>().attackDamage = GetComponent<FireElemental>().attackDamage;
        }
        //페이즈 2
        else
        {
            //소횐되는 burn의 속도가 1.5배 증가
            GameObject _burn = Instantiate(burnPrefab, burnpos, Quaternion.identity, treepleBurn);
            _burn.GetComponent<RotateAroundBurn>().turnSpeed *= 1.5f;
            _burn.GetComponent<RotateAroundBurn>().attackDamage = GetComponent<FireElemental>().attackDamage * 1.5f;
            //_burn.GetComponent<RotateAroundBurn>().burnLifetime *= 1.5f;
        }

        //burn의 스텍 카운트 증가
        burnCount++;
       
    }

    public void RotateBurn()
    {
        StartCoroutine("TreepleBurnStart");
    }

    IEnumerator TreepleBurnStart()
    {
        yield return new WaitForSeconds(.5f);
        //burn이 소환되는 스텍 저장소 (treepleBurn) 의 자식(burn)이 3개(3스텍)가 되면 TreepleBurn 시작
        for (int i = 0; i < treepleBurn.childCount; i++)
        {
            //RotateAroundBurn의 isTrun이 true가 되면 TreeplBurn 발동
            treepleBurn.GetChild(i).GetComponent<RotateAroundBurn>().isTurn = true;    
        }

        //스킬이 발동되면 스텍 초기화
        burnCount = resetCount;
    }

    public void endAttack()
    {
        fireElemental.isAttack = false;
        fireElemental.eState = EnemyState.Idle;
        fireElemental.enemyAnime.SetInteger("eState", (int)fireElemental.eState);  
    }
}
