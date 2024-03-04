using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursorImage : Interaction
{
    public bool isDraw = false;
    public cursorImages enableCursor;
    public cursorImages disableCursor;

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
                player.isObject = true;
                if (theDistance < actionDis)
                {
                    CursorManager.instance.SetCursurImage(enableCursor);
                }
                else
                {
                    CursorManager.instance.SetCursurImage(disableCursor);
                }
            }
        }
    }

    //타겟에서 마우스가 나갔을 경우
    public override void DontAction()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }

    private void OnDestroy()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }
}
