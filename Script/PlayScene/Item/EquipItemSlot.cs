using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipItemSlot : MonoBehaviour , IPointerClickHandler
{
    private Inventory inven;
    private Equipment equipment;

    //장착한 장비의 이미지
    public GameObject equipItemImage;
    //선택된 슬롯 이미지
    public GameObject selectImage;

    //슬롯의 타입(무기는 무기슬롯에만 들어올 수 있게 하기 위함)
    public EquipType slotType;
    public int slotTypeNum;

    //현재 장착중인 아이템
    private EquipItem equipItem;

    public void SetData()
    {
        inven = Inventory.instance;
        equipment = Equipment.instance;
        slotTypeNum = (int)slotType;
    }

    public void SelectEquipSlot()
    {       
        if (equipment.equipItems[slotTypeNum] == null)
        {
            return;
        }

        selectImage.SetActive(true);
    }

    public void UnEquipItemSlot()
    {
        inven.AddItem(equipment.equipItems[slotTypeNum]);
        equipment.UnEquipItem(slotTypeNum);
        /*
        equipItemImage.GetComponent<Image>().sprite = null;
        equipItemImage.SetActive(false);
        selectImage.SetActive(false);
        */
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //현재 슬롯이 빈 칸이 아닐 때만 작동
            if (equipment.equipItems[slotTypeNum] != null)
            {
                UnEquipItemSlot();
                AudioManager.instance.PlayeSound("unEquipSound");
            }
        }
    }
}
