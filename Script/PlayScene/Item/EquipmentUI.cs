using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    private Equipment equipment;

    //장착창 UI
    public GameObject equipUI;
    private RectTransform equipUIrect;
    private bool isOpen;

    private InventoryUI invenUI;

    //장착슬롯을 관리하는 부모 오브젝트
    public Transform equipItems;

    //장착한 장비가 저장되는 장비 슬롯
    private EquipItemSlot[] equipSlot;

    private EquipItem equipItem;

    
    //장착한 장비의 생성 위치
    public Transform weaponSlot;
    public Transform shieldSlot;

    //선택된 슬롯의 번호
    public int slotNum = -1;

    private Vector3 resetPos;

    public RectTransform levelTitle;

    private void Awake()
    {
        Equipment.instance.SetEquipData = SetData;
        LanguageOption.SortingUI += SortingUI;
        PlayerStatus.stausUI += SortingUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        resetPos = new Vector3(0, 0, 0);
        
        //CloseUI();       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }
        if (GameSceneMenu.isMenuOpen)
        {
            return;
        }

        ToggleUI();
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            equipUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            equipUI.SetActive(!equipUI.activeSelf);

            if(equipUI.activeSelf == false)
            {
                DeSelectAllSlots();
            }
        }
        */
    }

    public void SetData()
    {
        equipment = Equipment.instance;
        equipSlot = equipItems.GetComponentsInChildren<EquipItemSlot>();

        invenUI = GetComponent<InventoryUI>();
        equipUIrect = equipUI.GetComponent<RectTransform>();

        CloseUI();

        for (int i = 0; i < equipSlot.Length; i++)
        {
            int _slotNum = i;
            //0번 슬롯의 버튼 이벤트에 SelectSlot 메서드 등록. 슬롯의 번호는 슬롯의 아이템타입
            equipSlot[i].GetComponent<Button>().onClick.AddListener(delegate { SelectEquipSlot(_slotNum); });
            equipSlot[i].SetData();
        }

        equipment.AddUpdateEquip += AddEquipItem;
        equipment.RemoveUpdateEquip += RemoveEquipItem;
    }

    public void SortingUI()
    {
        RectTransform[] rec = levelTitle.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < rec.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rec[i]);
        }
    }

    public void ToggleUI()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeSelectAllSlots();
            CloseUI();
        }
        

        if (Input.GetKeyDown(KeyCode.M))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                equipUIrect.anchoredPosition = new Vector3(0, 0, 0);
            }
            else
            {
                DeSelectAllSlots();
                equipUIrect.anchoredPosition = new Vector3(0, 1080f, 0);
            }
        }
    }
    public void CloseUI()
    {
        //equipUI.SetActive(false);
        DeSelectAllSlots();
        isOpen = false;
        equipUIrect.anchoredPosition = new Vector3(0, 1080f, 0);
    }

    //아이템 장착 시 상태 업데이트(슬롯 이미지 및 장착 장비 외형)
    public void AddEquipItem(EquipItem _equipItem)
    {
        //장착한 아이템 타입을 int형으로 저장
        int equipType = (int)_equipItem.equipType;

        //장착한 장비 타입에 맞는 장비 슬롯을 찾는다.
        for (int i = 0; i < equipSlot.Length; i++)
        {
            //equipSlot[i].slotNum : i번째 equipSlot의 slotNum(슬롯타입넘버)
            //현재 받은 받은 장비가 무기(1번)이면 equipSlot[1].slotNum 일때 True가 되어 슬롯1번에 값을 세팅하게 된다.
            if (equipSlot[i].slotTypeNum == equipType)
            {
                //장비 이미지 보여주기
                equipSlot[i].equipItemImage.SetActive(true);
                equipSlot[i].equipItemImage.GetComponent<Image>().sprite = _equipItem.itemImege;             
            }
        }

        //장착하는 장비 실제 게임화면에서 구현
        if(equipType == (int)EquipType.Weapon)
        {
            GetComponent<PlayerStatusUI>().EquipWeapon(_equipItem.itemImege);
            GameObject weapon = Instantiate(_equipItem.equipItemObject, weaponSlot.transform.position , Quaternion.identity, weaponSlot);
            //캐릭터가 장비를 들고 있을때 자연스럽게 보이기 위해 장비의 rotate offset 추가
            weapon.transform.localPosition = resetPos;
            weapon.transform.localEulerAngles = resetPos;
            weaponSlot.transform.localScale = weapon.transform.localScale;
            weapon.transform.localScale = new Vector3(1, 1, 1);
            weaponSlot.transform.localEulerAngles = _equipItem.offset;
        }
        else if(equipType == (int)EquipType.Shield)
        {
            GameObject shield = Instantiate(_equipItem.equipItemObject, shieldSlot.transform.position, Quaternion.identity, shieldSlot);
            shield.transform.localPosition = resetPos;
            shield.transform.localEulerAngles = resetPos;
            shieldSlot.transform.localEulerAngles = _equipItem.offset;
        }
    }

    public void RemoveEquipItem(EquipItem _equipItem)
    {
        
        //탈착한 아이템 타입을 int형으로 저장
        int equipType = (int)_equipItem.equipType;

        //탈착한 장비 타입에 맞는 장비 슬롯을 찾는다. ex) 무기는 무기슬롯 
        for (int i = 0; i < equipSlot.Length; i++)
        {
            //장비 슬롯과 장비 타입이 맞는다면 ex) 무기슬롯 == 무기타입           
            if (equipSlot[i].slotTypeNum == equipType)
            {
                //장비 이미지 제거 
                equipSlot[i].equipItemImage.GetComponent<Image>().sprite = null;
                equipSlot[i].equipItemImage.SetActive(false);            

                //장비슬롯이 선택되어 있으면 선택 이미지 초기화
                if (equipSlot[i].selectImage.activeSelf == true)
                {
                    equipSlot[i].selectImage.SetActive(false);
                }
            }
        }

        //실제 게임내에서도 탈착된 장비 제거
        if (equipType == (int)EquipType.Weapon)
        {
            //장착 중인 무기UI 이미지 제거
            GetComponent<PlayerStatusUI>().UnEquipWeapon();

            //weaponSlot의 자식 오브젝트(장착중인 장비)를 가져온다.
            Transform[] weapons = weaponSlot.GetComponentsInChildren<Transform>();

            //GetComponentsInChildren 사용시 부모 오브젝트도 가져올 수 있기 때문에 태그를 사용하여 걸러준다.
            foreach (var weapon in weapons)
            {
                if (weapon.CompareTag("Item"))
                {
                    Destroy(weapon.gameObject);
                }
            }
            weaponSlot.transform.localEulerAngles = resetPos;
            weaponSlot.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (equipType == (int)EquipType.Shield)
        {
            Transform[] shields = shieldSlot.GetComponentsInChildren<Transform>();

            foreach (var shield in shields)
            {
                if (shield.CompareTag("Item"))
                {
                    Destroy(shield.gameObject);
                }
            }
            shieldSlot.transform.localEulerAngles = resetPos;
            shieldSlot.transform.localScale = new Vector3(1, 1, 1);
        }

        //선택 초기화
        slotNum = -1;
    }

    public void SelectEquipSlot(int _slotNumber)
    {
        invenUI.DeSelectAllSlots();

        //아이템을 장착하면 아이템 이미지가 켜진다 = 아이템이 없으면 아이템 이미지가 꺼져있다.
        //아이템이 없으면 장비슬롯을 선택하지 못하게 한다.
        if (equipSlot[_slotNumber].equipItemImage.activeSelf == false)
        {
            Debug.Log("장착장비 x");
            DeSelectAllSlots();
            return;
        }
        

        //현재 선택된 슬롯과 새로 선택한 슬롯이 같을 경우
        if (slotNum == _slotNumber)
        {
            //셀렉 이미지꺼주고 선택된 슬롯 초기화
            equipSlot[slotNum].selectImage.SetActive(false);
            slotNum = -1;
        }
        //다른 슬롯을 선택한 경우
        else
        {
            //전에 선택한 슬롯이 있으면
            if (slotNum != -1)
            {
                //전에 선택한 슬롯의 셀렉 이미지 오프
                equipSlot[slotNum].selectImage.SetActive(false);
            }

            //새로 선택한 슬롯을 현재 슬롯으로 전환
            slotNum = _slotNumber;
            //slotNumber = itemSlot[_slotNumber].slotNum;

            //현재 슬롯의 셀렉 이미지 활성화
            equipSlot[slotNum].SelectEquipSlot();
        }
    }

    public void DeSelectAllSlots()
    {
        for (int i = 0; i < equipSlot.Length; i++)
        {
            equipSlot[i].selectImage.SetActive(false);
            slotNum = -1;
        }
    }

    public bool UIOpenCheck()
    {
        return isOpen;
    }

    private void OnDestroy()
    {
        LanguageOption.SortingUI -= SortingUI;
        PlayerStatus.stausUI -= SortingUI;
    }
}
