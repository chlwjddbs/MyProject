using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOutline : Interaction
{
    //아웃라인 메테리얼
    public Material[] outlineMaterial;

    //오리지널 메테리얼
    public Material[] orignMaterial;

    public bool isDraw = false;

    private MeshRenderer outline;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<MeshRenderer>();
    }


    public override void OnMouseOver()
    {
        DoAction();
    }

    //Interaction 참조 : 마우스가 타겟을 포지션 했을때
    public override void DoAction()
    {
        if (isDraw)
        {
            //아이템이 UI와 겹치거나 플레이어가 액션 중일 때 outline 미표시
            if (PlayerController.isUI | PlayerController.isAction)
            {
                DontAction();
                return;
            }
            else
            {
                outline.materials = outlineMaterial;
            }
        }
    }

    //타겟에서 마우스가 나갔을 경우
    public override void DontAction()
    {
        //그렇지 않으면 오리지널 출력
        outline.materials = orignMaterial;

    }
}
