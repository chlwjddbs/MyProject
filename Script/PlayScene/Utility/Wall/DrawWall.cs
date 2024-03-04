using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWall : MonoBehaviour
{
    //상태가 바뀔벽 (부모)
    public GameObject Wall;
    private MeshRenderer wallMesh;


    //불투명, 반투명 벽 메트리얼 
    public List<Material> wallMaterials;
    // Start is called before the first frame update

    public bool isHide = false;
    public bool isPassHide = false;
    
    void Start()
    {
        wallMesh = Wall.GetComponent<MeshRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWall();
       
    }

    public void ChangeWall()
    {
        if (isHide & !isPassHide)
        {
            wallMesh.material = wallMaterials[1]; //반투명 메트리얼 적용
        }
        else
        {
            wallMesh.material = wallMaterials[0]; //불투명 메트리얼 적용
        }
    }

   

    /*
    private void OnTriggerEnter(Collider other)
    {
        changeState.material = wallMaterials[1];
    }

    private void OnTriggerExit(Collider other)
    {
        changeState.material = wallMaterials[0];
    }
    */
}
