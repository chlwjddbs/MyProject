using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour
{
    public float theDistance;
    public float actionDis = 5f;

    protected PlayerController player;


    private void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
    }


    // Update is called once per frame
    public virtual void Update()
    {
        
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            theDistance = PlayerController.CheckDistance;
        }
        else
        {
            theDistance = 100f;
        }
        
        //theDistance = PlayerController.CheckDistance;
    }

    //OnMouseOver가 아닌 업데이트에서 처리시 모든 아이템에 DoAction이 적용 된다.
    public virtual void OnMouseOver()
    {
        if (theDistance < actionDis)
        {
            DoAction();
        }
    }

    private void OnMouseExit()
    {
        DontAction();
    }

    public virtual void DoAction()
    {

    }

    public virtual void DontAction()
    {
        player.isObject = false;
    }

    
}
