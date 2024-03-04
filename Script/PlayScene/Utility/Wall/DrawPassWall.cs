using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPassWall : MonoBehaviour
{
    public List<GameObject> wall;

    public List<Material> wallMaterials;
   

    // Update is called once per frame
    void Update()
    {
        HidePassWall();
        DrawPassWall2();
    }

    private void HidePassWall()
    {
        if (wall[0].GetComponentInChildren<DrawWall>().isPassHide || wall[0].GetComponentInChildren<DrawWall>().isHide)
        {
            wall[1].GetComponent<MeshRenderer>().material = wallMaterials[1];
        }
        else
        {
            wall[1].GetComponent<MeshRenderer>().material = wallMaterials[0];
        }
    }

    private void DrawPassWall2()
    {
        if (wall[0].GetComponent<MeshRenderer>().enabled == false)
        {
            wall[1].GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            wall[1].GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            wall[0].GetComponentInChildren<DrawWall>().isPassHide = true;     
        }
    }

  

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wall[0].GetComponentInChildren<DrawWall>().isPassHide = false;
        }
    }
}
