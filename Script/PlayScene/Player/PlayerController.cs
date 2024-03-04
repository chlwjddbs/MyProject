using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //메인 카메라 가져오기.
    private Camera cameraView;
    public Vector3 mouseDir;

    //씬에서 오브젝트 보기
    public GameObject SceneView;

    public Animator playerAnime;

    public PlayerStatus playerStatus;

    //이동보정 오프셋값
    public Vector3 posOffset;

    //쿼터뷰 시점에서 플레이어 로테이션에 카메라가 영향 받기 않기 위해 플레이어를 분리하여 관리.
    public Transform player;

    //플레이어와 마우스의 거리를 계산해 걸을지 뛸지를 판단하는 거리.
    [HideInInspector]public float runRange = 5f;
    //플레이어가 달릴시 걷기
    public bool isRun = false;

    //오브젝트와의 거리
    public static float CheckDistance = 100f;

    //캐릭터 상태
    public PlayerState pState = PlayerState.Idle;

    //마우스 포인트가 UI인지 판정
    public static bool isUI = false;

    //마우스 포인트가 상호 작용 가능한 오브젝트인지 판정
    public bool isObject = false;

    //2023.04.05 기준 공격과 점프가 나눠져 있을 이유가 없으므로 액션으로 통합. ( + 캐스팅 추가 )
    //public bool isAttack = false;
    //public bool isJump = false;

    public static bool isAction = false;
    public static bool isCasting = false;

    private string ActionState = "ActionState";

    public AnimatorOverrideController myOverrideAnim;

    //스킬 이펙트 생성 위치
    public Transform effectPos;

    /*
    //클릭한 지점으로 이동 구현 테스트
    public bool isMove = false;
    private Vector3 test;
    */
    // Start is called before the first frame update
    void Start()
    {
        //메인 카메라는 Carera.main으로 씬내에 있는 메인 카메라를 가져올 수 있다.
        cameraView = Camera.main;
        SceneView.SetActive(false);
        //playerAnime = GetComponentInChildren<Animator>();
        //playerStatus = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }

        if (!DataManager.instance.isSet)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(isUI);
        }
        if (playerStatus.isDeath)
        {
            return;
        }

        //공격 중이거나 점프중일때는 다른 동작 막기
        //if (!isAttack && !isJump)
        if(!isAction)
        {
            Vector3 mousePos = Input.mousePosition;
            LookAtMouse(mousePos);
            Move(mouseDir);
            Attack();
            Jump();
        }


        switch (pState)
        {
            case PlayerState.Idle:
                ResetState();
                break;

            case PlayerState.Walk:
                transform.Translate(mouseDir.normalized * (playerStatus.moveSpeed / 2) * Time.deltaTime, Space.World);
                break;

            case PlayerState.Run:
                //걷는 도중에는 달리기로 상태 전환이 가능하지만 달리는 중에는 걷는 상태로 전환 불가
                isRun = true;
                transform.Translate(mouseDir.normalized * playerStatus.moveSpeed * Time.deltaTime, Space.World);
                break;

            case PlayerState.Attack:
                //공격 중 방향 변경 가능
                Vector3 mousePos_a = Input.mousePosition;
                LookAtMouse(mousePos_a);
                break;

            case PlayerState.Jump:
                //점프중엔 방향 변경 불가능
                transform.Translate(mouseDir.normalized * playerStatus.moveSpeed * Time.deltaTime, Space.World);
                break;

            case PlayerState.Action:
                
                break;

            case PlayerState.Casting:
                //캐스팅 중 방향 변경 가능
                if (isCasting)
                {
                    Vector3 mousePos_c = Input.mousePosition;
                    LookAtMouse(mousePos_c);
                }
                break;

            case PlayerState.Death:
                break;
        }
        
    }

    public void ResetState()
    {
        isObject = false;
        //isAttack = false;
        isAction = false;
        isRun = false;
    }

    private void Move(Vector3 mouseDir)
    {
        //마우스 포인트가 UI 일때 이동 금지
        if (isUI)
        {
            SetState(PlayerState.Idle);
            return;
        }

        //마우스 우클릭 유지시
        if (Input.GetMouseButton(1))
        {
            //마우스와 플레이어의 거리가 runRange미만이고 달리지 않고 있을때
            //달리는 도중에는 걷기로 모션 전환 불가
            if (mouseDir.magnitude < runRange && !isRun)
            {
                //걷는다.
                SetState(PlayerState.Walk);
            }

            //마우스와 플레이어의 거리가 runRange이상 일때 달린다.
            else
            {
                SetState(PlayerState.Run);
            }
        }

        //우클릭을 끝낼시 그자리에서 멈춘다.
        if (Input.GetMouseButtonUp(1))
        {
            SetState(PlayerState.Idle);
        }
    }

    private void Attack()
    {
        //마우스 포인트가 UI이거나 item일때 공격 금지
        if (isUI | isObject)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //isAttack = true;
            isAction = true;
            SetState(PlayerState.Attack);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //isJump = true;
            isAction = true;
            SetState(PlayerState.Jump);
        }
    }

   
    private void LookAtMouse(Vector3 mousePos)
    {
        //스크린에서 월드로 레이를 그림
        Ray ray = cameraView.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {     
            //Vector3 mouseDir;
            //히트 지점을 마우스 방향으로 저장
            mouseDir = new Vector3(hit.point.x, player.position.y, hit.point.z) - player.position;
            
            //플레이어의 앞 방향을 마우스 방향으로 지정
            player.forward = mouseDir;
              
            //hit된 오브젝트가 true이면 플레이어와의 거리를 표시해준다.
            if (CheckTag(hit.transform.tag))
            {
                CheckDistance = mouseDir.magnitude;
            }
            else
            {
                //그렇지 않으면 거리 초기화
                CheckDistance = 100f;
            }
        }    

    }


    //마우스에 닿은 오브젝트 판별
    private bool CheckTag(string hitTag)
    {
        if (hitTag == "Item") return true;
        if (hitTag == "Npc") return true;
        if (hitTag == "Object") return true;
        return false;
    }

    //플레이어 상태를 받아와 플레이어 애니메이션을 변경
    public void SetState(PlayerState _state)
    {
        //같은 상태를 받을 시 애니메이션 끊기지 않게 리턴해준다.
        if (pState == _state)
        {
            return;
        }     
        pState = _state;
        playerAnime.SetInteger("pState", (int)pState);

    }

    //스킬 사용시 스킬의 애니메이션 클립을 받아 사용할 스킬에 맞는 애니메이션 재생
    public void SetAnime(AnimationClip _clip)
    {
        //현재 적용중인 클립과 새로 들어온 클립이 다른 클립일 때만 실행
        if (ActionState != _clip.name)
        {
            //ActionState의 클립을 변경해준다.
            myOverrideAnim[ActionState] = _clip;

            //ovrride controller로 바뀌지 않았을 때만 실행
            if (playerAnime.runtimeAnimatorController != myOverrideAnim)
            {
                playerAnime.runtimeAnimatorController = myOverrideAnim;
            }
        }  
    }

    public void SetAttackAnime(AnimationClip _clip)
    {
        myOverrideAnim["Player_Attack"] = _clip;
      
        if (playerAnime.runtimeAnimatorController != myOverrideAnim)
        {
            playerAnime.runtimeAnimatorController = myOverrideAnim;
        }

    }

    public void SetCastMotion(float _motionSlect)
    {
        playerAnime.SetFloat("MotionSelect", _motionSlect);
    }

    public void SetActionSpeed(float _acSpeed)
    {
        playerAnime.SetFloat("ActionSpeed", _acSpeed);
    }
}

//플레이어 상태를 나타내는 enum
public enum PlayerState
{
    Idle,               //0
    Walk,
    Run,
    Attack,
    Jump,
    Action,             //5
    Casting,
    Death = 100,

}

//클릭한 곳으로 이동
/*
private void clickMove(Vector3 mousePos)
{

    if ((transform.position - mousePos).magnitude < 0.1f)
    {
        isMove = false;
    }
    transform.localPosition -= (transform.localPosition - mousePos).normalized * Time.deltaTime * 5f;

}
*/

//턱 올라가기
/*
if ((hit.point.y - (transform.position.y + posOffset.y)) > 0f && (hit.point.y - (transform.position.y + posOffset.y)) <= 1f)
{
    mouseDir = new Vector3(hit.point.x- player.position.x, hit.point.y - (transform.position.y + posOffset.y), hit.point.z- player.position.z);

}
else
{
    mouseDir = new Vector3(hit.point.x, player.position.y, hit.point.z) - player.position;
}

클릭한 곳으로 이동하기 구현
if (Input.GetMouseButtonUp(0))
{
    test = hit.point;
    isMove = true;
    Debug.Log(test);
}

if (isMove)
{
    clickMove(test);
}
*/
