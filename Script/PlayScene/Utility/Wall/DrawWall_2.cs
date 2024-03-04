using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWall_2 : DrawWalls
{
    //상태가 바뀔벽 (부모)
    //public GameObject Wall;
    //public GameObject UnderWall;
    private MeshRenderer wallMesh;
    private MeshRenderer underWallMesh;

    //불투명, 반투명 벽 메트리얼 
    //public List<Material> wallMaterials;
    // Start is called before the first frame update

    //public bool isHide = false;
    //public bool isDraw = false;

    void Start()
    {
        wallMesh = Wall.GetComponent<MeshRenderer>();
        if (UnderWall != null)
            underWallMesh = UnderWall.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWall();
        DrawUnderWalls();
        DrawWalls();
    }

    public void ChangeWall()
    {
        if (isHide)
        {
            wallMesh.material = wallMaterials[1]; //반투명 메트리얼 적용
        }
        else
        {
            wallMesh.material = wallMaterials[0]; //불투명 메트리얼 적용
        }
    }

    public void DrawUnderWalls()
    {
        if (wallMesh.enabled == true)
        {
            if (UnderWall != null)
            {
                underWallMesh.enabled = true;
            }
        }
        else
        {
            if (UnderWall != null)
            {
                underWallMesh.enabled = false;
            }
        }
    }

    public void DrawWalls()
    {
        if (isDraw && wallMesh.enabled == false)
        {
            wallMesh.enabled = true;
        }
        else if (!isDraw && wallMesh.enabled == true)
        {
            wallMesh.enabled = false;
        }
    }
}
