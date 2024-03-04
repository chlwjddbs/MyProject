using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    //시야 거리
    public float sightRaduis = float.MaxValue;

    //시야내 물체를 감지하는 범위
    public float sightRange = 50f;

    //시야 각도
    [Range(0, 360)]
    public float sightAngle;

    //시야에 닿는 장애물
    public LayerMask targetMask, obstacleMask, checkFadeMask, wallsMask, objectMask;

    //시야에 닿은 오브젝트 리스트
    public List<Transform> viewTarget = new List<Transform>();

    public float maskCutawayDis = 0.1f;

    public float meshResolution;

    Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public Vector3 sigthOffset;
    public Vector3 drawOffset;

    public List<Transform> visibleWalls = new List<Transform>();

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    // Update is called once per frame
    void Update()
    {
        viewTargetAdd();      //범위 내 적 등록하기
        viewTargetRemove();   //범위 외 적 삭제하기
        //VisibleWall();        //벽 보기 : 시야에 들어온 벽만 보기
        //VisibleWall_2();
        VisibleWall_3();
        //VisibleUnderWall(); //UnderWall 보기
        VisibleParticle();
        //SightWall();        //벽 보기 : 시야를 가리는 벽 반투명 처리하기
        //SightWall_2();
        HideWall();
        VisibleObject();
    }

    private void LateUpdate()
    {
        visibleWalls.Clear();
        DrawSight(); //시야 그리기
    }

    //적 탐지하기
    private void viewTargetAdd()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 targetMask(Enemy)를 targetView에 찾아온다.
        Collider[] targetView = Physics.OverlapSphere(transform.position, sightRaduis, targetMask);

        //targetView = 감지된 타겟의 배열
        for (int i = 0; i < targetView.Length; i++)
        {

            //감지된 타겟 구하기
            Transform target = targetView[i].transform;

            //감지된 타겟의 방향 구하기
            Vector3 targetDir = (target.position - (transform.position + sigthOffset)).normalized;

            // 플레이어와 forward와 target이 이루는 각이 설정한 각도이고 감지 범위 이내에 있다면
            if (Vector3.Angle(transform.forward, targetDir) < sightAngle / 2)
            {
                //플레이어와 타겟의 거리를 저장.
                float targetdDis = Vector3.Distance(transform.position + sigthOffset, target.transform.position);

                //플레이어 위치에서 타겟 거리 만큼 레이를 쏜다.
                //그 후 레이에 장애물이 닿지 않았고
                //if (!Physics.Raycast(transform.position, targetDir, targetdDis, obstacleMask))
                Debug.DrawRay(transform.position + sigthOffset, targetDir * targetdDis,Color.blue);

                //타겟이 시야거리 밖으로 나가면 타겟의 랜더러를 꺼준다.
                if (targetdDis > sightRange)
                {
                    if (target.GetComponent<Enemy>().RenderBox.activeSelf == true)
                    {
                        target.GetComponent<Enemy>().RenderBox.SetActive(false);
                    }
                    //보여지고 있는 목록에 있는 적이면 삭제
                    if (viewTarget.Contains(target))
                    {
                        viewTarget.Remove(target);
                    }
                }
                //타겟이 시야거리 내에 있을 때
                else
                {
                    //타겟으로 레이를 발사해 레이가 벽에 닿지 않는다면
                    if (!Physics.Raycast(transform.position + sigthOffset, targetDir, targetdDis, wallsMask))
                    {
                        //리스트에 추가되지 않은 타겟이면 리스트에 추가하고 랜더러를 켜준다.
                        if (!viewTarget.Contains(target))
                        {
                            target.GetComponent<Enemy>().RenderBox.SetActive(true);
                            viewTarget.Add(target);
                        }
                    }
                    //레이가 장애물에 닿았고
                    //else if (Physics.Raycast(transform.position, targetDir, targetdDis, obstacleMask))
                    else if (Physics.Raycast(transform.position + sigthOffset, targetDir, targetdDis, wallsMask))
                    {
                        //리스트에 타겟이 있다면 타겟에서 삭제하고 랜더러를 꺼준다.
                        if (viewTarget.Contains(target))
                        {
                            target.GetComponent<Enemy>().RenderBox.SetActive(false);
                            viewTarget.Remove(target);
                        }
                    }
                }
            }
            /*
            //감지범위지만 시야각에 들어오지 않은 적 타겟에서 제거
            else
            {
                target.GetComponent<Enemy>().RenderBox.SetActive(false);
                viewTarget.Remove(target);
            }
            */
        }
    }

    //탐지 범위 밖 이너미 제거
    private void viewTargetRemove()
    {
        //현재 감지중인 이너미를 저장하는 리스트 생성
        List<Transform> target = new List<Transform>();

        //감지중인 이너미
        Collider[] targetView = Physics.OverlapSphere(transform.position, sightRaduis, targetMask);

        for (int i = 0; i < targetView.Length; i++)
        {

            target.Add(targetView[i].transform);

        }

        //현재 감지중인 리스트와 이미 감지되어 등록된 이너미를 비교.
        for (int j = 0; j < viewTarget.Count; j++)
        {
            //이너미가 감지범위 밖으로 나가 없어졌으면 감지중인 이너미 리스트에서 빠지게 된다.
            //하지만 이미 등록중인 이너미 리스트를 그대로이기 떄문에
            //이미 등록된 이너미와 현재 감지중인 이너미를 비교하여,
            //감지중인 이너미 리스트에 등록된 이너미가 없으며녀 제거한다.
            if (!target.Contains(viewTarget[j]))
            {
                viewTarget.Remove(viewTarget[j]);
            }
        }
        /*
            for (int i = 0; i < viewTarget.Count; i++)
        {

            if ((transform.position - viewTarget[i].position).magnitude > sightRaduis)
            {
                viewTarget.Remove(viewTarget[i]);
            }
        }
        */
    }

    //시야에 들어온 벽만 보기
    /*
    public void VisibleWall()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 벽들을 찾는다.
        Collider[] targetWall = Physics.OverlapSphere(transform.position, sightRaduis, obstacleMask);

        for (int i = 0; i < targetWall.Length; i++)
        {
            //감지된 벽 구하기
            Transform wall = targetWall[i].transform;

            //감지된 벽의 방향 구하기
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //플레이어 시야 앵글안에 벽이 있다면
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //플레이어와 벽의 거리
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //플레이어에서 벽 방향으로 벽과의 거리 만큼 레이캐스트를 쏴 벽을 받아온다.
                //RaycastHit[] hit = Physics.RaycastAll(transform.position, wallDir, wallDis, obstacleMask);
                RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);

                if (wall.name == "1")
                {
                    Debug.DrawRay(transform.position + sigthOffset, wallDir * wallDis, Color.blue, 1f);
                    Debug.Log(hit.Length);
                }

                //충돌한 벽이 2개 이상일때
                if (hit.Length > 1f)
                {
                    //충돌한 벽이 2개 이상이지만 시야에 들어온 벽이라면 보여준다.
                    if (visibleWalls.Contains(wall))
                    {
                        if (wall.GetComponent<MeshRenderer>().enabled == false)
                        {
                            wall.GetComponent<MeshRenderer>().enabled = true;
                            if (wall.GetChild(0).name == "UnderWall")
                            {
                                wall.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (wall.GetComponent<MeshRenderer>().enabled == true)
                        {
                            //시야에 들어오지 않은 벽의 매시랜더러를 꺼준다. (안보이게 함)
                            wall.GetComponent<MeshRenderer>().enabled = false;
                            if (wall.GetChild(0).name == "UnderWall")
                            {
                                wall.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                            }
                        }
                    }
                }
                else
                {
                    if (wall.GetComponent<MeshRenderer>().enabled == false)
                    {
                        wall.GetComponent<MeshRenderer>().enabled = true;
                        if (wall.GetChild(0).name == "UnderWall")
                        {
                            wall.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                }

            }
        }
    }

    public void VisibleWall_2()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 벽들을 찾는다.
        Collider[] targetWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("Wall_2"));

        for (int i = 0; i < targetWall.Length; i++)
        {
            //감지된 벽 구하기
            Transform wall = targetWall[i].transform;

            //감지된 벽의 방향 구하기
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //플레이어 시야 앵글안에 벽이 있다면
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //플레이어와 벽의 거리
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //플레이어에서 벽 방향으로 벽과의 거리 만큼 레이캐스트를 쏴 벽을 받아온다.
                RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);

                if (wall.name == "1")
                {
                    Debug.DrawRay(transform.position + sigthOffset, wallDir * wallDis, Color.blue, 1f);
                    Debug.Log(hit.Length);
                }

                //충돌한 벽이 2개 이상일때
                if (hit.Length > 1f)
                {
                    //충돌한 벽이 2개 이상이지만 시야에 들어온 벽이라면 보여준다.
                    if (visibleWalls.Contains(wall))
                    {
                        if (wall.GetComponent<DrawWall_2>().isDraw == false)
                        {
                            wall.GetComponent<DrawWall_2>().isDraw = true;
                        }

                    }
                    else
                    {
                        if (wall.GetComponent<DrawWall_2>().isDraw == true)
                        {
                            //시야에 들어오지 않은 벽의 매시랜더러를 꺼준다. (안보이게 함)
                            wall.GetComponent<DrawWall_2>().isDraw = false;
                        }

                    }
                }
                else
                {
                    if (wall.GetComponent<DrawWall_2>().isDraw == false)
                    {
                        wall.GetComponent<DrawWall_2>().isDraw = true;

                    }


                }

            }
        }
    }
    */
    public void VisibleWall_3()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 벽들을 찾는다.
        Collider[] targetWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("Wall"));

        for (int i = 0; i < targetWall.Length; i++)
        {
            //감지된 벽 구하기
            Transform wall = targetWall[i].transform;

            //감지된 벽의 방향 구하기
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //플레이어 시야 앵글안에 벽이 있다면
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //플레이어와 벽의 거리
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //벽과의 거리가 시야범위보다 멀어지면 시야에 들어오지 않았다고 판정한다.
                if (wallDis > sightRange)
                {
                    if (wall.GetComponent<DrawWalls>().isDraw == true)
                    {
                        wall.GetComponent<DrawWalls>().isDraw = false;
                    }
                }
                else
                {
                    //플레이어에서 벽 방향으로 벽과의 거리 만큼 레이캐스트를 쏴 벽을 받아온다.
                    RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);

                    if (wall.name == "1")
                    {
                        Debug.DrawRay(transform.position + sigthOffset, wallDir * wallDis, Color.blue, 1f);
                        Debug.Log(hit.Length);
                        Debug.Log(wallDis);
                    }

                    //충돌한 벽이 2개 이상일때
                    if (hit.Length > 1f)
                    {
                        //충돌한 벽이 2개 이상이지만 시야에 들어온 벽이라면 보여준다.
                        if (visibleWalls.Contains(wall))
                        {
                            if (wall.GetComponent<DrawWalls>().isDraw == false)
                            {
                                wall.GetComponent<DrawWalls>().isDraw = true;
                            }
                        }
                        else
                        {
                            if (wall.GetComponent<DrawWalls>().isDraw == true)
                            {
                                //시야에 들어오지 않은 벽의 매시랜더러를 꺼준다. (안보이게 함)
                                wall.GetComponent<DrawWalls>().isDraw = false;
                            }
                        }
                    }
                    else
                    {
                        if (wall.GetComponent<DrawWalls>().isDraw == false)
                        {
                            wall.GetComponent<DrawWalls>().isDraw = true;
                        }
                    }
                }
            }
        }
    }

    public void VisibleObject()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 오브젝트들을 찾는다.
        Collider[] objects = Physics.OverlapSphere(transform.position, sightRaduis, objectMask);

        for (int i = 0; i < objects.Length; i++)
        {
            //감지된 오브젝트 구하기
            Transform _object = objects[i].transform;

            //감지된 오브젝트의 방향 구하기
            Vector3 _objectDir = (_object.position - (transform.position + sigthOffset)).normalized;

            //플레이어 시야 앵글안에 오브젝트이 있다면
            if (Vector3.Angle(transform.forward, _objectDir) < sightAngle / 2)
            {
                //플레이어와 오브젝트의 거리
                float _objectDis = Vector3.Distance(transform.position + sigthOffset, _object.transform.position);

                if (_objectDis > sightRange)
                {
                    if (_object.GetComponent<DrawOutline>() != null)
                    {
                        _object.GetComponent<DrawOutline>().isDraw = true;
                    }

                    if (_object.GetComponent<SetCursorImage>() != null)
                    {
                        _object.GetComponent<SetCursorImage>().isDraw = true;
                    }
                }
                else
                {
                    //플레이어에서 오브젝트의 방향으로 오브젝트와의 거리 만큼 레이캐스트를 쏴 오브젝트를 받아온다.
                    RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, _objectDir, _objectDis, wallsMask);

                    if (_object.name == "1")
                    {
                        Debug.DrawRay(transform.position + sigthOffset, _objectDir * _objectDis, Color.blue, 1f);
                        Debug.Log(hit.Length);
                    }

                    //충돌한 벽이 1개 이상일때
                    if (hit.Length >= 1f)
                    {
                        if (_object.GetComponent<DrawOutline>() != null)
                        {
                            _object.GetComponent<DrawOutline>().isDraw = false;
                        }

                        if (_object.GetComponent<SetCursorImage>() != null)
                        {
                            _object.GetComponent<SetCursorImage>().isDraw = false;
                        }
                    }
                    else
                    {
                        if (_object.GetComponent<DrawOutline>() != null)
                        {
                            _object.GetComponent<DrawOutline>().isDraw = true;
                        }

                        if (_object.GetComponent<SetCursorImage>() != null)
                        {
                            _object.GetComponent<SetCursorImage>().isDraw = true;
                        }
                    }
                }
            }
        }
    }
    public void VisibleParticle()
    {
        //플레이어 주변의 파티클을 찾는다.
        Collider[] particles = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("Particle"));
        for (int i = 0; i < particles.Length; i++)
        {
            //감지된 파티클 구하기
            Transform particle = particles[i].transform;

            //감지된 파티클의 방향 구하기
            Vector3 ptcDir = (particle.position - (transform.position + sigthOffset)).normalized;

            //플레이어 시야 각안에 파티클이 있다면
            if (Vector3.Angle(transform.forward, ptcDir) < sightAngle / 2)
            {
                //플레이어와 파티클 거리
                float ptcDis = Vector3.Distance(transform.position + sigthOffset, particle.transform.position);

                if (ptcDis > sightRange)
                {
                    if (particle.GetChild(0).gameObject.activeSelf == true)
                    {
                        particle.GetChild(0).gameObject.SetActive(false);
                    }
                }
                else
                {
                    //if (!Physics.Raycast(transform.position, ptcDir, ptcDis, obstacleMask))
                    if (!Physics.Raycast(transform.position + sigthOffset, ptcDir, ptcDis, wallsMask))
                    {
                        Debug.DrawRay(transform.position + sigthOffset, ptcDir * ptcDis, Color.blue, 1f);
                        //파티클 방향으로 레이를 쏴서 장애물에 걸리지 않는다면 파티클 활성화
                        if (particle.GetChild(0).gameObject.activeSelf == false)
                        {
                            particle.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        //파티클 방향으로 레이를 쏴서 장애물에 걸리면 파티클 비활성화
                        if (particle.GetChild(0).gameObject.activeSelf == true)
                        {
                            particle.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    /* VisibleUnderWall()
    public void VisibleUnderWall()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 벽들을 찾는다.
        Collider[] targetUnderWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("UnderWall"));

        //UnderWall은 높이가 낮아 y값 보정 없이 Ray를 쏘면 일반벽도 같이 맞기 때문에 y값을 보정해준다.
        Vector3 rayPos = transform.position+sigthOffset;

        for (int i = 0; i < targetUnderWall.Length; i++)
        {
            //감지된 벽 구하기
            Transform wall = targetUnderWall[i].transform;

            //감지된 벽의 방향 구하기
            Vector3 wallDir = (wall.position - transform.position).normalized;

            //플레이어 시야 앵글안에 벽이 있다면
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //플레이어와 벽의 거리
                float wallDis = Vector3.Distance(rayPos, wall.transform.position);

                //플레이어에서 벽 방향으로 벽과의 거리 만큼 레이캐스트를 쏴 벽을 받아온다.
                RaycastHit[] hit = Physics.RaycastAll(rayPos, wallDir, wallDis , 1 << LayerMask.NameToLayer("UnderWall"));
                Debug.DrawRay(rayPos, wall.position - transform.position, Color.black);
                //충돌한 벽이 2개 이상일때

                if (hit.Length > 1f)
                {
                    //벽의 매시랜더러를 꺼준다. (안보이게 함)
                    wall.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    wall.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }
    */

    /*
    //벽 반투명 처리
    public void SightWall()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 벽 트리거를 찾는다.
        Collider[] checkWall = Physics.OverlapSphere(transform.position, sightRaduis, checkFadeMask);

        for (int i = 0; i < checkWall.Length; i++)
        {
            //감지된 벽 트리거 구하기
            Transform wall = checkWall[i].transform;

            //감지된 벽의 방향 구하기
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //플레이어 시야 앵글안에 트리거가 있다면
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //플레이어와 벽의 거리
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //플레이어에서 벽 방향으로 벽과의 거리 만큼 레이캐스트를 쏴 트리거를 받아온다.
                //if (!Physics.Raycast(transform.position, wallDir, wallDis, obstacleMask))
                if (!Physics.Raycast(transform.position + sigthOffset, wallDir, wallDis, wallsMask))
                {
                    //시야에 트리거가 감지도면 벽을 반투명 처리
                    wall.GetComponentInParent<DrawWall>().isHide = true;
                }
                else
                {
                    //시야에서 트리거가 없어지면 원상복구
                    wall.GetComponentInParent<DrawWall>().isHide = false;
                    //Debug.Log(wall.parent.name);
                }


            }
        }
    }

    public void SightWall_2()
    {
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 벽 트리거를 찾는다.
        Collider[] checkWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("CheckWall_2"));

        for (int i = 0; i < checkWall.Length; i++)
        {
            //감지된 벽 트리거 구하기
            Transform wall = checkWall[i].transform;

            //감지된 벽의 방향 구하기
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //플레이어 시야 앵글안에 트리거가 있다면
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //플레이어와 벽의 거리
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //플레이어에서 벽 방향으로 벽과의 거리 만큼 레이캐스트를 쏴 트리거를 받아온다.
                //if (!Physics.Raycast(transform.position, wallDir, wallDis, obstacleMask))
                if (!Physics.Raycast(transform.position + sigthOffset, wallDir, wallDis, wallsMask))
                {
                    //시야에 트리거가 감지도면 벽을 반투명 처리
                    wall.GetComponentInParent<DrawWalls>().isHide = true;
                }
                else
                {
                    //시야에서 트리거가 없어지면 원상복구
                    wall.GetComponentInParent<DrawWalls>().isHide = false;
                }


            }
        }
    }
    */

    public void HideWall()
    {
        //Debug.DrawRay(transform.position + sigthOffset, Vector3.forward * 123f, Color.blue, 1f);
        //플레이어기준으로 sightRaduis크기의 OverlapSphere를 그려 그 안에 들어온 checkWall들을 찾는다.
        Collider[] checkWalls = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("CheckWall"));

        for (int i = 0; i < checkWalls.Length; i++)
        {
            //감지된 checkWall
            Transform checkWall = checkWalls[i].transform;

            //감지된 checkWall의 부모가 현재 시야내에 들어온 목록에 있다면
            if (visibleWalls.Contains(checkWall.parent))
            {
                //checkWall 방향 구하기
                Vector3 wallDir = (checkWall.position - (transform.position + sigthOffset)).normalized;

                //플레이어와 checkWall의 거리
                float wallDis = Vector3.Distance(transform.position + sigthOffset, checkWall.transform.position);

                //checkWall 방향으로 레이를 쏴서
                RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);
                
                //아무것도 맞지 않았다면
                //(wllsMask, 즉 실제 벽에 레이가 닿지 않았다는것은 플레이어가 벽 뒤에서 바라 봤기 때문에 레이가 벽까지 닿지 않은 것임으로 벽을 반투명처리해준다.)
                if (hit.Length == 0)
                {
                    if (!checkWall.GetComponentInParent<DrawWalls>().isHide)
                    {
                        //벽을 반투명하게 바꿔준다.
                        checkWall.GetComponentInParent<DrawWalls>().isHide = true;
                    }
                }
                else
                {
                    //하나 이상 레이에 들어오면 들어온 벽이 무엇인지 판별해 준다.
                    for (int j = 0; j < hit.Length; j++)
                    {
                        //만약 hit[j].transform.gameObject가 checkWall의 부모라면 벽의 정면을 보고 있는것이기 때문에 벽을 불투명하게 한다.
                        if (hit[j].transform.gameObject == checkWall.parent.gameObject)
                        {
                            if (checkWall.GetComponentInParent<DrawWalls>().isHide)
                            {
                                //벽을 반투명하게 바꿔준다.
                                checkWall.GetComponentInParent<DrawWalls>().isHide = false;
                            }
                            break;
                        }
                        //checkWall의 부모가 걸리지 않았다면 벽의 뒤를 보고 있고, 다른 벽에 걸린것이지만 시야내에 들어온 벽이기 때문에 반투명 처리를 해준다.
                        else if (j == hit.Length - 1)
                        {
                            if (!checkWall.GetComponentInParent<DrawWalls>().isHide)
                            {
                                //벽을 반투명하게 바꿔준다.
                                checkWall.GetComponentInParent<DrawWalls>().isHide = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!checkWall.GetComponentInParent<DrawWalls>().isHide)
                {
                    //벽을 반투명하게 바꿔준다.
                    checkWall.GetComponentInParent<DrawWalls>().isHide = true;
                }
            }
        }
    }

    //아이템 버리기
    public bool Dropable(Vector3 DropPos)
    {
        DropPos.y = 0.5f;
        //DropPos까지의 거리를 구함
        float DropDis = Vector3.Distance(transform.position + sigthOffset, DropPos);

        //DropPos로의 방향을 구함
        Vector3 Dropdir = (DropPos - (transform.position + sigthOffset)).normalized;

        //플레이어위치로부터 DropPos까지 레이를 쏴서 장애물에 걸리지 않으면
        if (!Physics.Raycast(transform.position + sigthOffset, Dropdir, DropDis, wallsMask))
        {
            //버리기 가능
            return true;
        }
        else
        {
            //그리지 않으면 버리기 불가능
            return false;
        }
    }

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {

        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    private void DrawSight()
    {
        //sightAngle을 meshResolution의 갯수만큼 쪼갠다.
        int stepCount = Mathf.RoundToInt(sightAngle * meshResolution);
        float stepAngleSize = sightAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo prevViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            //sightAngle내 쪼개진 시야각도를 각각 ViewCast에 넘준다.
            float angle = transform.eulerAngles.y - sightAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // i가 0이면 prevViewCast에 아무 값이 없어 정점 보간을 할 수 없으므로 건너뛴다.
            if (i != 0)
            {
                bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                // 둘 중 한 raycast가 장애물을 만나지 않았거나 두 raycast가 서로 다른 장애물에 hit 된 것이라면(edgeDstThresholdExceed 여부로 계산)
                if (prevViewCast.hit != newViewCast.hit || (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
                {
                    Edge e = FindEdge(prevViewCast, newViewCast);

                    // zero가 아닌 정점을 추가함
                    if (e.PointA != Vector3.zero)
                    {
                        viewPoints.Add(e.PointA);
                    }

                    if (e.PointB != Vector3.zero)
                    {
                        viewPoints.Add(e.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            prevViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * maskCutawayDis;
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        public Transform visibleWall;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle, Transform _wall)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
            visibleWall = _wall;
        }
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        //angle각의 방향을 받아온다.
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
       // List<Transform> checkList = new List<Transform>();

        //각 방향으로 레이를 쏘아 wallMask가 히트 되었다면
        if (Physics.Raycast(transform.position + drawOffset, dir, out hit, sightRaduis, wallsMask))
        {
            //보이는 벽에 추가해준다.
            if (!visibleWalls.Contains(hit.transform))
            {
                visibleWalls.Add(hit.transform);
            }
                //hit.transform.GetComponent<MeshRenderer>().enabled = false;

                //hit된 벽의 내용을 저장한다.
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, hit.transform);
        }
        else
        {
            //hit되지 않았으면 flase와 함께 시야의 끝거리를 저장한다.
            return new ViewCastInfo(false, transform.position + dir * sightRaduis, sightRaduis, globalAngle, null);
        }

       
      
}

    public struct Edge
    {
        public Vector3 PointA, PointB;
        public Edge(Vector3 _PointA, Vector3 _PointB)
        {
            PointA = _PointA;
            PointB = _PointB;
        }
    }

    Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = minAngle + (maxAngle - minAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceed)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new Edge(minPoint, maxPoint);
    }
}
