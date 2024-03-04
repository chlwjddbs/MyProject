using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : Interaction
{
    public Item item;

    public override void DoAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Inventory.instance.AddItem(item);
            
            Debug.Log(item.name + " : æ∆¿Ã≈€ »πµÊ" ) ;
            Destroy(gameObject);
        }
    }
}
