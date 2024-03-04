using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichBall : MonoBehaviour
{
    //공격 시작 위치
    private Vector3 startPos;
    //목표 대상 위치
    private Vector3 targetPos;
    //Slerp를 위한 center값
    private Vector3 center;
    //공격이 끝나기까지 걸리는 시간(LichBall이 날아가는데 걸리는 시간)
    public float endAttackTime =1f;
    //리치볼이 생성된 시간
    private float startTime;

    private bool isTarget = false;

    public float parabolaHeight;

    private GameObject player;

    private float attackDamage;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {       
        if(isTarget == false)
        {
            return;
        }
        targetPos = player.transform.position;
        center = (startPos + targetPos) * 0.5f;
        center -= new Vector3(0, 1f * parabolaHeight, 0);
        Vector3 startCenter = startPos - center;
        Vector3 targetCenter = targetPos - center;
        float complete = (Time.time - startTime) / endAttackTime;       
        transform.position = Vector3.Slerp(startCenter, targetCenter, complete);
        transform.position += center;

        if(complete > 1f)
        {
            Destroy(gameObject);
        }
    }

    public void GetTargetPos(Vector3 _targetPos,GameObject _target,float _attackDamage)
    {
        player = _target;
        targetPos = _targetPos;      
        startTime = Time.time;
        startPos = transform.position;
        isTarget = true;
        attackDamage = _attackDamage;
        
    }

    private void OnTriggerEnter(Collider other)
    {     
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);
        }
    }
}
