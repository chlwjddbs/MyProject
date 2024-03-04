using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWall_3 : DrawWalls
{
    /*
    public float eraseTime;
    private float countDonw;

    //0이면 불투명, 1이면 반투명
    private float wallMaterialNum;
    public GameObject Drawoverwall;
    public GameObject MiniMap;
    private bool isDrawMiniMap = false;
    */

    //public List<Material> eraseMaterials;

    //private int test;
    //public bool isErase;
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
    }

    public void ChangeWall()
    {
        //if(!isErase)
        //if (isDraw) : isDraw 판정을 하지 않아도 시야에 들어오지 않으면 isHide가 판정이 되지 않아 없어도 무방하다.
        {
            //메테리얼을 직접 비교하면 사용중인 메테리얼에 instance가되어 비교가 되지 않기 때문에 wallMaterialNum을 선언해 현재 메테리얼을 체크해준다.
            if (isHide && wallMaterialNum == 0)
            {
                wallMaterialNum = 1;
                Wall.material = wallMaterials[1]; //반투명 메트리얼 적용
            }
            else if (!isHide && wallMaterialNum == 1)
            {
                wallMaterialNum = 0;
                Wall.material = wallMaterials[0]; //불투명 메트리얼 적용
            }
        }
    }

    public void DrawWall()
    {
        //벽에 시야에서 벗어났다 다시 들어왔지만 아직 지워지지 않은 상태라면 eraseTime을 리셋 시켜주어야 한다.
        if(isDraw && countDonw < eraseTime)
        {
            countDonw = eraseTime;
            UnderWall.enabled = true;
            //isErase = false;
        }
        if (isDraw && Wall.enabled == false)
        {
            countDonw = eraseTime;
            //isErase = false;
            Wall.enabled = true;
            UnderWall.enabled = true;
        }               
        else if(!isDraw && Wall.enabled == true)
        {
            EraseWall();
        }
    }

    //시야에 나간즉시 없애주지 않고 잔상을 남겨준다.
    public void EraseWall()
    {
        countDonw -= Time.deltaTime;
        if (countDonw <= 0)
        {
            UnderWall.enabled = false;
            isDraw = false;
            Wall.enabled = false;
        }

        /*
        if(UnderWall.enabled == true)
        {
            UnderWall.enabled = false;
        }
        */

        /*
       isErase = true;
        countDonw -= Time.deltaTime;

        //Debug.Log(test);
        if ((countDonw > 0))
        {
            if (countDonw <= eraseTime - (eraseTime * ((float)(test) / (float)(eraseMaterials.Count))))
            {
                Wall.material = eraseMaterials[test];
                test++;
                UnderWall.enabled = false;
            }
        }
        else
        {
            isDraw = false;
            Wall.enabled = false;
            
            countDonw = eraseTime;
            test = 0;
        }
        */
    }

    public void DrawMiniMap()
    {
        if(isDraw && !isDrawMiniMap)
        {
            isDrawMiniMap = true;
            MiniMap.SetActive(true);
        }
    }

    public void DrawOverWall()
    {
        if (Drawoverwall != null)
        {
            if (isHide && Drawoverwall.activeSelf == true)
            {
                Drawoverwall.SetActive(false);
            }
            else if (!isHide && Drawoverwall.activeSelf == false)
            {
                Drawoverwall.SetActive(true);
            }
        }
    }
}
