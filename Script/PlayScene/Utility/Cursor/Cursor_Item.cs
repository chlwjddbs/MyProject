using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_Item : Interaction
{
    public bool isDraw = false;

    // Start is called before the first frame update

    public override void OnMouseOver()
    {
        DoAction();
    }

    //Interaction 참조 : 타겟과 플레이어의 거리가 actionDis 이내이고 마우스가 타겟을 포지션 했을때
    public override void DoAction()
    {
        if (isDraw)
        {
            //마우스가 UI를 가르킬 때
            if (PlayerController.isUI | PlayerController.isAction)
            {
                //마우스가 나갔다고 판정한다.
                DontAction();
                return;
            }
            else
            {
                if (theDistance < actionDis)
                {
                    //Cursor.SetCursor(CursorManager.instance.itemOverCursor_Enable, CursorManager.instance.hotSpot, CursorMode.Auto);
                    //Cursor.SetCursor(CursorManager.instance.itemOverCursor_Enable, CursorManager.instance.hotSpot, CursorMode.Auto);
                    CursorManager.instance.SetCursur(CursorManager.instance.itemOverCursor_Enable);
                }
                else
                {
                    CursorManager.instance.SetCursur(CursorManager.instance.itemOverCursor_Disable);
                }
            }
        }
    }

    //타겟에서 마우스가 나갔을 경우
    public override void DontAction()
    {
        CursorManager.instance.ResetCursor();
    }
}
