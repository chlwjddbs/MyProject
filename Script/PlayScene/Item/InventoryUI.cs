using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Inventory inven;

    //실제 인벤토리 창으로 쓰일 오브젝트
    public GameObject InvenUI;

    //인벤토리UI On,Off 상태
    private RectTransform InvenUIrect;
    private bool isOpen = false;

    private EquipmentUI equipUI;

    //아이템 슬롯들을 가지고 있는 부모오브젝트 Items를 받아옴
    public Transform Items;

    //Items에서 가져온 슬롯들을 정리할 배열 생성
    [HideInInspector] public ItemSlot[] itemSlot;

    //현재 선택된 슬롯의 번호
    public int slotNum = -1; //-1 : 선택된 슬롯의 번호가 없을 경우

    public invenState invenState = invenState.stay;

    public GameObject Player;

    public Sound[] InvenSound;

    private void Awake()
    { 
        Inventory.instance.SetInvenData += SetData;
    }

    private void Start()
    {
        //CloseUI();
        foreach (var s in InvenSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
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
    }

    /*
    //아이템을 획득하는 등 인벤토리에 변화가 있을 시 인벤토리 업데이트
    public void UpdateInven()
    {
        //아이템을 제거시 아이템 카운트 만큼 포문이 돌면 지워진 아이템 슬롯까지 도달하지 못한다.
        //예를들어 8개의 아이템 중 하나의 아이템을 제거하면 아이템 카운트는 7이된다.
        //그런데 아이템 카운트 만큼 포문을 돌리게 되면 7번째 슬롯까지 슬롯을 비우고
        //제거된 8번째 슬롯은 제거되지 못해 UI상에서는 존재 하는걸로 인식하게 된다.
        //그러므로 전체 초기화(itemSlot.Length)를 진행하거나
        //아이템 카운트 +1(inven.items.Count+1)만큼 초기화를 진행 시켜줘야 한다.
        for (int i = 0; i < inven.items.Count + 1; i++)
        {
            itemSlot[i].RemoveItemSlot();
            AllSlotsReset();
        }

        //인벤토리에 있는 아이템 만큼 다시 등록
        for (int i = 0; i < inven.items.Count; i++)
        {
            itemSlot[i].SetItemSlot();
        }
    }
    */

    public void SetData()
    {
        inven = Inventory.instance;
        equipUI = GetComponent<EquipmentUI>();
        InvenUIrect = InvenUI.GetComponent<RectTransform>();
        CloseUI();

        //Items의 자식들 = itemSlot의 갯수
        //Items의 자식들 중 ItemSlot을 찾아 배열에 저장 
        itemSlot = Items.GetComponentsInChildren<ItemSlot>();

        //인벤토리 사이즈는 인벤토리 슬롯의 갯수
        inven.invenSize = itemSlot.Length;

        //슬롯 번호 지정
        for (int i = 0; i < itemSlot.Length; i++)
        {
            //매개변수를 가진 OnClick 이벤트 
            //itemSlot[i].GetComponent<Button>().onClick.AddListener(() => SelectSlot(i));
            int slotNum = i;
            //매개 변수가 있는 메서드 연결시 람다 함수를 이용하여 연결
            //itemSlot[i].GetComponent<Button>().onClick.AddListener(() =>SelectSlot(index));
            //delegate 함수 이용

            itemSlot[i].GetComponent<Button>().onClick.AddListener(delegate { SelectSlot(slotNum); });
            itemSlot[i].slotNum = i;
            itemSlot[i].SetData();
        }

        //인벤토리 업데이트 액션함수에 등록
        inven.AddUpdateUI += AddItemUI;
        inven.RemoveUpdateUI += DeSelectSlot;
        inven.CheckUseableSlot += CheckSlot;
        inven.GoodsOverlap += GoodsOverlap;

        if (DataManager.instance.newGame)
        {

        }
        else
        {
            inven.LoadData();
            for (int i = 0; i < inven.items.Length; i++)
            {
                if (inven.items[i] != null)
                {
                    SetData(i);
                }
            }
        }
    }

    public void ToggleUI()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
            {
                //InvenUI.SetActive(false);
                DeSelectAllSlots();
                CloseUI();
                AudioManager.instance.PlayExSound("InvenClose");
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //인벤토리가 직접 꺼지면 다양한 버그가 발생하여 위치를 옮기는 방식으로 변경
            /*InvenUI.SetActive(!InvenUI.activeSelf);          
            if (InvenUI.activeSelf == false)
            {
                DeSelectAllSlots();
            }
            */

            isOpen = !isOpen;
            if (isOpen)
            {
                InvenUIrect.anchoredPosition = new Vector3(0, 0, 0);
                AudioManager.instance.PlayExSound("InvenOpen");
            }
            else
            {
                DeSelectAllSlots();
                InvenUIrect.anchoredPosition = new Vector3(1920f, 0, 0);
                AudioManager.instance.PlayExSound("InvenClose");
            }
        }
    }

    public void AddItemUI(int _slotNum)
    {
        itemSlot[_slotNum].SetItemSlot();
    }

    public void SetData(int _slotNum)
    {
        if (inven.invenItems[_slotNum].slotItem != null)
        {
            /*
            itemSlot[_slotNum].SetItemSlot();
            inven.invenItems[_slotNum].quantity = DataManager.instance.userData.invenItem[_slotNum].quantity;
            itemSlot[_slotNum].QuantityItem();
            itemSlot[_slotNum].CheckUseSlot();
            
            */
            for (int i = 0; i < DataManager.instance.userData.invenItem[_slotNum].quantity; i++)
            {
                itemSlot[_slotNum].SetItemSlot();
            }
            //*/
        }
    }

    //인벤토리에 추가되는 아이템이 중첩 가능한 아이템일 때 중첩 가능한지 체크
    public void GoodsOverlap(int _slotNum, Item _item)
    {
        if (_item.itemType == ItemType.Potion)
        {
            inven.isOverlap = itemSlot[_slotNum].PotionOverlapable();
        }
    }

    //아이템이 들어갈 슬롯의 상태 체크
    public void CheckSlot(Item _receiveItem)
    {
        for (int i = 0; i < inven.invenSize; i++)
        {
            //아이템이 들어있는 슬롯 발견 시
            if (itemSlot[i].HavedItem() != null)
            {
                //슬롯에 들어 있는 아이템이 획든한 아이템과 같고 중첩 가능한 상태이면
                if (_receiveItem == itemSlot[i].HavedItem() && itemSlot[i].PotionOverlapable() == true)
                {
                    //포션 중첩 가능
                    invenState = invenState.able;
                    inven.UseableSlot = true;
                    break;
                }

                //슬롯에 들어 있는 아이템이 받은 아이템과 같지만 중첩이 불가능한 상태라면
                else if (_receiveItem == itemSlot[i].HavedItem() && itemSlot[i].PotionOverlapable() == false)
                {
                    //for문이 돌기 때문에 모든 슬롯을 검사하게 된다.
                    //그때 앞의 슬롯이 true 판정이 나도 뒤에 슬롯이 false가 되면
                    //인벤토리에 해당 아이템이 중첩 가능한 상태이지만 false를 반환하여 중첩이 불가능하다고 판단 할 수 있다.
                    //그렇기 때문에 앞선 검증에서 true 처리를 받는다면 continue를 통해 건너뛰어
                    //한번 받은 true 판정이 false로 바뀌지 않도록 해준다.
                    if (invenState == invenState.able)
                    {
                        continue;
                    }
                    //인벤토리 슬롯을 다해도 중첩 가능한 인베토리 슬롯이 없으면 중첩 불가능(false) 판정
                    else
                    {
                        invenState = invenState.enable;
                        inven.UseableSlot = false;
                    }
                }
            }
            //같은 이름의 아이템이 없다면 처음 획득하는 아이템으로 inventory에서 빈 슬롯을 찾도록 함.
            else
            {
                inven.UseableSlot = false;
            }
        }
    }


    /*
    public void RemoveItemUI(int _slotNum, Item _item)
    {
        itemSlot[_slotNum].RemoveItemSlot();
    }
    */


    public void CloseUI()
    {
        //InvenUI.SetActive(false);
        DeSelectAllSlots();
        isOpen = false;
        InvenUIrect.anchoredPosition = new Vector3(1920f, 0, 0);
    }


    //슬롯 선택
    public void SelectSlot(int _slotNumber)
    {
        equipUI.DeSelectAllSlots();

        //아이템이 없는 슬롯의 넘버는 -1이다.
        //그럴경우 _slotNumber을 받아올 경우 새로 선택한 슬롯이 현재 선택한 빈 슬롯으로 저장되어 아이템이 없어도 슬롯을 선택했다고 판정한다.
        //빈 슬롯을 눌러놓고 그 자리에 아이템을 받아오고 아이템을 선택하면 셀렉이미지를 바로 보여주지 않는다.

        //아이템이 없는 슬롯 선택시 슬롯 선택 판정을 해주지 않는다.
        if (itemSlot[_slotNumber].HavedItem() == null)
        {
            DeSelectAllSlots();
            return;
        }

        //현재 선택된 슬롯과 새로 선택한 슬롯이 같을 경우
        if (slotNum == _slotNumber)
        {
            //셀렉 이미지꺼주고 선택된 슬롯 초기화
            itemSlot[slotNum].selectImage.SetActive(false);
            slotNum = -1;
        }
        //다른 슬롯을 선택한 경우
        else
        {
            //전에 선택한 슬롯이 있으면
            if (slotNum != -1)
            {
                //전에 선택한 슬롯의 셀렉 이미지 오프
                itemSlot[slotNum].selectImage.SetActive(false);
            }

            //새로 선택한 슬롯을 현재 슬롯으로 전환
            slotNum = _slotNumber;
            //slotNumber = itemSlot[_slotNumber].slotNum;

            //현재 슬롯의 셀렉 이미지 활성화
            itemSlot[slotNum].SelectSlot();
        }
    }

    public void DeSelectSlot(int _slotNumber)
    {
        itemSlot[_slotNumber].selectImage.SetActive(false);
        slotNum = -1;
    }

    //모든 슬롯 리셋
    public void DeSelectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectImage.SetActive(false);
            slotNum = -1;
        }
    }

    public bool UIOpenCheck()
    {
        return isOpen;
    }
}


public enum invenState
{
    stay,   //반복문을 토해 able과 enable 상태를 판정하기 때문에 반복문 전 대기중인 상태
    able,   //인벤토리 내에 획득 할 아이템이 들어갈 슬롯이 있다고 판정.
    enable, //인벤토리 내에 획득 할 아이템이 들어갈 슬롯이 없다고 판정.
}
