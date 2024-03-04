using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDoorWall : DrawWalls
{
    public DrawArchWall archWall;
    public bool isDrawDoor;
    public bool isHideDoor;

    void Start()
    {
        if (MiniMap != null)
        {
            MiniMap.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWall();
        DrawWall();
        DrawOverWall();
        if (MiniMap != null)
        {
            DrawMiniMap();
        }

        CheckDraw();
        CheckHide();
    }

    public void ChangeWall()
    {
        {
            //메테리얼을 직접 비교하면 사용중인 메테리얼에 instance가되어 비교가 되지 않기 때문에 wallMaterialNum을 선언해 현재 메테리얼을 체크해준다.
            if (isHideDoor && wallMaterialNum == 0)
            {
                wallMaterialNum = 1;
                Wall.material = wallMaterials[1]; //반투명 메트리얼 적용
            }
            else if (!isHideDoor && wallMaterialNum == 1)
            {
                wallMaterialNum = 0;
                Wall.material = wallMaterials[0]; //불투명 메트리얼 적용
            }
        }
    }

    public void DrawWall()
    {
        if (isDrawDoor && countDonw < eraseTime)
        {
            countDonw = eraseTime;
            UnderWall.enabled = true;
        }
        if (isDrawDoor && Wall.enabled == false)
        {
            countDonw = eraseTime;
            Wall.enabled = true;
            UnderWall.enabled = true;
        }
        else if (!isDrawDoor && Wall.enabled == true)
        {
            EraseWall();
        }
    }

    public void EraseWall()
    {
        countDonw -= Time.deltaTime;
        if (countDonw <= 0)
        {
            UnderWall.enabled = false;
            Wall.enabled = false;
        }
    }

    public void DrawMiniMap()
    {
        if (isDrawDoor && !isDrawMiniMap)
        {
            isDrawMiniMap = true;
            MiniMap.SetActive(true);
        }
    }

    public void DrawOverWall()
    {
        if (Drawoverwall != null)
        {
            if (isHideDoor && Drawoverwall.activeSelf == true)
            {
                Drawoverwall.SetActive(false);
            }
            else if (!isHideDoor && Drawoverwall.activeSelf == false)
            {
                Drawoverwall.SetActive(true);
            }
        }
    }

    public void CheckDraw()
    {
        if (!isDrawDoor && archWall.isDraw)
        {
            isDrawDoor = true;
        }
        else if(isDrawDoor && !archWall.isDraw)
        {
            isDrawDoor = false;
        }
    }

    public void CheckHide()
    {
        if (!isHideDoor && archWall.isHide)
        {
            isHideDoor = true;
        }
        else if (isHideDoor && !archWall.isHide)
        {
            isHideDoor = false;
        }
    }
}
