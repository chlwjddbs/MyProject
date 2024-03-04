using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_ActivateObject : Interaction
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
            if (PlayerController.isUI | PlayerController.isAction)
            {
                DontAction();
                return;
            }
            else
            {
                if (theDistance < actionDis)
                {
                    player.isObject = true;
                    //Cursor.SetCursor(CursorManager.instance.activateObjectCursor, CursorManager.instance.hotSpot, CursorMode.Auto);
                    CursorManager.instance.SetCursur(CursorManager.instance.activateObjectCursor);
                    
                }
                else
                {
                    //Cursor.SetCursor(CursorManager.instance.DisabledObjectCursor, CursorManager.instance.hotSpot, CursorMode.Auto);
                    CursorManager.instance.SetCursur(CursorManager.instance.DisabledObjectCursor);
                }
            }
        }
    }

    //타겟에서 마우스가 나갔을 경우
    public override void DontAction()
    {
        player.isObject = false;
        //Cursor.SetCursor(CursorManager.instance.orginCursor, Vector2.zero, CursorMode.Auto);
        CursorManager.instance.ResetCursor();
    }

    private void OnDisable()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }
}
