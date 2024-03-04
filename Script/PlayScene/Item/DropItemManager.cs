using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    public static DropItemManager instance;

    [System.Serializable]
    public class AddDropItem
    {
        public Item fieldItem;
        public int quantity;
        public Vector3 itemPos;

        public AddDropItem(AddItem _test1,Vector3 _itemPos)
        {
            fieldItem = _test1.item;
            quantity = _test1.quantity;
            itemPos = _itemPos;

        }
    }

    private void Awake()
    {
        instance = this;
    }
}
