using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrawOrder : MonoBehaviour
{
    public List<GameObject> UIlist = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //if (UIlist != null)
        {
            for (int i = 0; i < UIlist.Count; i++)
            {
                UIlist[i].transform.SetSiblingIndex(i);
                Debug.Log(UIlist[i].transform.GetSiblingIndex());
                Debug.Log(UIlist[i].name);
            }
        }
    }

}
