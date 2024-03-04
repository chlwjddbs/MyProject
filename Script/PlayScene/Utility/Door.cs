using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private float Door_Y;
    public float closeSpeed = 5f;
    public bool isHold = false;
    public GameObject Player;
    private float targetDis;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //targetDis = Vector3.Distance(transform.position, Player.transform.position);
        DoorEuler();
        HoldDoor();
        

        
        /*
        if (!isHold)
        {
            
            //CloseDoor();
        }
        */
    }

    private void DoorEuler()
    {
        if (transform.rotation.y < 0)
        {
            Door_Y = 360 - transform.rotation.eulerAngles.y;
        }
        else
        {
            Door_Y = transform.rotation.eulerAngles.y;
        }
    }
    private void HoldDoor()
    {
        if (Door_Y >= 89.5 && isHold)
        {
            //GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            //GetComponent<BoxCollider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void CloseDoor()
    {
        if (transform.rotation.y < 0)
        {
            if (-Door_Y < -1f)
            {
                //transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * closeSpeed);
                GetComponent<Rigidbody>().AddForce(transform.forward * closeSpeed);
            }
        }
        else if(transform.rotation.y > 0)
        {
            if (Door_Y > 1f)
            {
                //transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * -closeSpeed);
                GetComponent<Rigidbody>().AddForce(-transform.forward * closeSpeed);
            }
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ºÙÀ½");
        if (collision.transform.CompareTag("Player"))
        {
            isHold = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            isHold = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("¶³¾îÁü");
        if (collision.transform.CompareTag("Player"))
        {
            isHold = false;
        }
    }
    */
}
