using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseSkill : MonoBehaviour
{
    public GameObject skill_A;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && skill_A != null)
        {
            Skill_A();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F))
        {

        }
        else if (Input.GetKeyDown(KeyCode.G))
        {

        }
    }

    public void Skill_A()
    {
        Debug.Log("A스킬 발동");
    }
}
