using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSequenceText : MonoBehaviour
{
    public string popupText;
    public float pupupCoolTIme = 4f;
    private bool isWrite =true;
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isWrite == true)
            {
                isWrite = false;
                SequenceText.instance.SetSequenceText(null, popupText);
                Invoke("resetCoolTime", pupupCoolTIme);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (isWrite == true)
            {
                isWrite = false;
                SequenceText.instance.SetSequenceText(null, popupText);
                Invoke("resetCoolTime", pupupCoolTIme);
            }
        }
    }

    private void resetCoolTime()
    {
        isWrite = true;
    }
}
