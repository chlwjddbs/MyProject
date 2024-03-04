using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Inventory inven;
    private Equipment equipment;
    private InventoryUI invenUI;
    private DropItemPopupUI dropUI;

    //슬롯에 등록된 아이템 이미지
    public Image itemImage;

    //슬롯이 선택되었을 때 보여질 이미지
    public GameObject selectImage;

    //현재 슬롯의 번호
    public int slotNum;

    [SerializeField]
    private Inventory.InvenItem itemInfo;

    //현재 슬롯에 등록된 아이템
    private Item item
    {
        get
        {
            if (inven.invenItems.TryGetValue(slotNum, out itemInfo))
            {
                return inven.invenItems[slotNum]?.slotItem;
            }
            else
            {
                return null;
            }
        }
        set { inven.invenItems[slotNum].slotItem = value; }
    }

    private Item tempItem;

    //아이템 드래그 시작 위치
    private Transform _startParent;

    //아이템 드래그시 이미지의 sort를 높이기 위한 임시 저장장소
    public GameObject Dragtemp;

    private bool isDrag = false;

    public DropItemManager dropItemSlot;

    //소모품 아이템 중첩갯수
    private int quantity
    {
        get { return inven.invenItems[slotNum].quantity; }
        set { inven.invenItems[slotNum].quantity = value; }
    }

    public TextMeshProUGUI quantityText;

    //슬롯을 사용 가능 여부
    public bool isUseable = true;
    //아이템 중첩 실행 가능 여부
    public bool isOverlap = false;
   
    public void SetData()
    {
        inven = Inventory.instance;
        equipment = Equipment.instance;
        dropItemSlot = DropItemManager.instance;

        //InventoryUI 스크립트의 경우 UIManager오브젝트로 관리되고 있다. 그러므로 ItemSlot의 부모 오브젝트인 UIManager로부터 찾는다.
        invenUI = GetComponentInParent<InventoryUI>();
        dropUI = GetComponentInParent<DropItemPopupUI>();
        quantityText.text = "";
        CheckUseSlot();
    }

    //플레이어가 획득하여 인벤토리에 등록된 아이템 정보를 아이템 슬롯과 연동해 준다.
    public void SetItemSlot()
    {
        //인벤토리에 아이템이 추가 되었을 때 실행된다.
        //아이템이 추가 됐으면 존재하고 있지만, 만약에라도 인벤토리에 아이템이 없다고 판정 할 시 아이템 이미지를 켜지 않도록 방어 코드 설계

        //인벤토리에 아이템이 있다면
        if (inven.items[slotNum] != null)
        {
            //가지고 있는 갯수 +1
            quantity++;

            //인벤토리에 있는 아이템이 포션이라면
            if (inven.items[slotNum].itemType == ItemType.Potion)
            {
                //현재 가지고 있는 아이템의 갯수를 체크하여 text표시방법 여부 확인
                QuantityItem();

                //가지고 있는 갯수가 한개라면 이번에 새로 등록된 포션이기 떄문에
                if (quantity == 1)
                {
                    //인벤토리에 들어있는 아이템을 가져와 슬롯에 연동
                    //item = inven.items[slotNum];
                    //아이템 이미지 보여주기
                    itemImage.gameObject.SetActive(true);
                    itemImage.sprite = item.itemImege;

                    isOverlap = true;
                }
                //아이템의 갯수가 아이템의 최대 중첩한도에 도달하면 해당 슬롯에는 더 이상 중첩이 불가능하다고 표시해준다.
                else if (quantity >= (item as Potion).ownershipLimit)
                {
                    //InventoryUI의 CheckSlot 함수 호출 시 반복문을 통해 모든 슬롯을 체크하고
                    //invenState가 able 일시 인벤토리내에 사용 가능한 슬롯이 존재한다고 판정하고
                    //enable일시 사용 가능한 슬롯이 없다고 판정한다.
                    //그렇기 때문에 중첩한도가 모두 차서 사용할 수 없는 슬롯이라고 enable 판정을 내리면 
                    //함수 호출 시 사용 가능한 슬롯이 있어도 반복문이 정상 작동하지 않기 때문에 대기 상태로 전환해준다.
                    invenUI.invenState = invenState.stay;

                    //quantityText.text = quantity.ToString();

                    isOverlap = false;
                }
            }
            //인벤토리에 있는 아이템이 포션을 제외한 아이템이라면
            else
            {
                //아이템 저장
                item = inven.items[slotNum];
                //아이템 이미지 보여주기
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.itemImege;
            }

            //아이템 추가 이후 인벤토리 상태 체크
            CheckUseSlot();
        }
    }

    //슬롯에서 아이템 제거
    public void RemoveItemSlot()
    {
        item = null;
        itemImage.gameObject.SetActive(false);
        itemImage.sprite = null;
        quantity = 0;
        QuantityItem();
        CheckUseSlot();
    }

    //슬롯 선택하기
    public void SelectSlot()
    {
        //선택한 슬롯에 아이템이 있는 경우에만 슬롯 선택을 가능하게 해준다.
        //아이템이 없을 경우 빈 슬롯이기 때문에 리턴 처리
        if (item == null)
        {
            return;
        }
        selectImage.SetActive(true);
    }

    public void UseItem()
    {
        if (item == null)
        {
            return;
        }

        quantity--;
        //사용하는 아이템이 포션일 때
        if (item.itemType == ItemType.Potion)
        {
            Debug.Log(inven.invenItems[slotNum].quantity);
            QuantityItem();
            //사용 후 남은 갯수가 없다면
            if (quantity <= 0)
            {
                tempItem = item;
                //인벤토리에서 정보 제거
                inven.RemoveItem(slotNum);
                //슬롯에서 정보 제거
                RemoveItemSlot();

                //사용 효과 발동
                tempItem.Use(slotNum);
            }
            else
            {
                item.Use(slotNum);
            }
        }
        //사용하는 아이템이 스킬북일 때
        else if(item.itemType == ItemType.SkillBook)
        {
            if(SkillBook.instance.isLearn(item as SkillItem))
            {
                tempItem = item;
                //인벤토리에서 정보 제거
                inven.RemoveItem(slotNum);
                //슬롯에서 정보 제거
                RemoveItemSlot();
                tempItem.Use(slotNum);
            }
            else
            {
                Debug.Log("이미 습득한 스킬 입니다.");
            }
        }

        //그 외 아이템 일 때
        else
        {
            tempItem = item;
            //인벤토리에서 정보 제거
            inven.RemoveItem(slotNum);
            //슬롯에서 정보 제거
            RemoveItemSlot();
            tempItem.Use(slotNum);
        }

        //아이템 사용 후 인벤토리 상태 체크
        CheckUseSlot();
    }

    public void QuantityItem()
    {
        if (quantity <= 1)
        {
            quantityText.text = "";
        }
        else
        {
            quantityText.text = quantity.ToString();
        }
        /*
        if (quantity <= 1)
        {
            quantityText.text = "";
        }
        else
        {
            quantityText.text = quantity.ToString();
        }
        */
    }

    public Item HavedItem()
    {
        return item;
    }


    //인벤토리 슬롯을 사용 가능한지, 소모품 중첩이 가능한 상태인지 체크.
    public void CheckUseSlot()
    {
        if (item == null)
        {
            isUseable = true;
            isOverlap = false;
            return;
        }
        if (item.itemType == ItemType.Potion)
        {
            if (quantity >= (item as Potion).ownershipLimit)
            {
                isUseable = false;
                isOverlap = false;
            }
            else
            {
                isUseable = true;
                isOverlap = true;
            }
        }
        else
        {
            isUseable = false;
            isOverlap = false;
        }
    }

    //현재 슬롯에 추가되는 아이템이 포션일 때 중첩 가능 여부 체크
    public bool PotionOverlapable()
    {
        //소지 한도에 여유가 있으면 true : 중첩 가능 상태
        if (isUseable == true && isOverlap == true)
        {
            return true;
        }
        //최대 소지 한도에 도달하면 false : 중첩 불가능 상태
        else if (isUseable == true && isOverlap == false)
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    public void DropItem()
    {
        RaycastHit hit;
        Vector3 hitpos;
        Vector3 DropPos;

        //마우스위치에서 레이캐스트를 쏴서
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, ~(1 << 10))) //플레이어는 제외 (플레이어 
        {
            //히트된 오브젝트가 water, obstacle , object가 아닐경우 아이템을 버릴 수 있게 한다.
            if (!hit.transform.CompareTag("Water") && !hit.transform.CompareTag("Obstacle") && !hit.transform.CompareTag("Object"))
            {
                hitpos = hit.point;
                Vector3 PlayerPos = new Vector3(invenUI.Player.transform.position.x, 1f, invenUI.Player.transform.position.z);
                if (invenUI.Player.GetComponentInChildren<PlayerSight>().Dropable(hitpos))
                {
                    //아이템이 포션이고 2개 이상 가지고 있다면
                    if (item.itemType == ItemType.Potion && quantity > 1)
                    {
                        //몇개를 버릴지 물어본다.
                        Debug.Log("몇개를 버리시겠습니까?");
                        dropUI.SetDropItemUI(slotNum, quantity, Input.mousePosition);
                    }
                    //아이템이 2개 미만인 포션이거나 포션이 아닐때
                    else
                    {
                        //현재 플에이어의 위치와 아이템을 버린 위치의 거리가 10f 미만일 때
                        if ((PlayerPos - hitpos).magnitude < 10f)
                        {
                            hitpos.y = 0.5f;
                            //버린 위치에 버릴 아이템을 생성하고    
                            Instantiate(item.FieldObject, hitpos, item.FieldObject.transform.rotation, dropItemSlot.transform);
                            //인벤토리에서 아이템을 정리한다.
                            inven.RemoveItem(slotNum);
                            RemoveItemSlot();
                        }
                        //거리가 10f 이상일 때
                        else
                        {
                            //10거리 보다 멀때 DropPos를 최대거리인 10으로 조정하여 10위치에 아이템을 버린다.
                            DropPos = PlayerPos + ((hitpos - PlayerPos).normalized * 10);
                            DropPos.y = 0.5f;
                            Instantiate(item.FieldObject, DropPos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                            //인벤토리에서 아이템을 정리한다.
                            inven.RemoveItem(slotNum);
                            RemoveItemSlot();
                        }

                        AudioManager.instance.PlayeSound("dropItem");
                    }
                }
            }
        }
    }

    public void DropPotion(int _quantity, Vector2 _DropPos)
    {
        if (_quantity <= 0)
        {
            return;
        }

        RaycastHit hit;
        Vector3 hitpos;
        Vector3 DropPos;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(_DropPos), out hit))
        {
            hitpos = hit.point;
            Vector3 PlayerPos = new Vector3(invenUI.Player.transform.position.x, 1f, invenUI.Player.transform.position.z);
            if ((PlayerPos - hitpos).magnitude < 10f)
            {
                hitpos.y = 0f;
                if (quantity - _quantity <= 0)
                {
                    Debug.Log("포션 버려");
                    GameObject _potion = Instantiate(item.FieldObject, hitpos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    inven.RemoveItem(slotNum);
                    RemoveItemSlot();
                }
                else
                {
                    GameObject _potion = Instantiate(item.FieldObject, hitpos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    quantity -= _quantity;
                    QuantityItem();
                    CheckUseSlot();
                }
            }
            else
            {
                DropPos = PlayerPos + ((hitpos - PlayerPos).normalized * 10);
                DropPos.y = 0;
                if (quantity - _quantity <= 0)
                {
                    GameObject _potion = Instantiate(item.FieldObject, DropPos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    inven.RemoveItem(slotNum);
                    RemoveItemSlot();
                }
                else
                {
                    GameObject _potion = Instantiate(item.FieldObject, DropPos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    quantity -= _quantity;
                    QuantityItem();
                    CheckUseSlot();
                }
            }


        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }
        isDrag = true;
        //Drag & Drop 구현시 Drop된 슬롯과 Drag한 슬롯의 아이템을 바꿔 주어야 한다.
        //Drop된 슬롯이 들어갈 위치(Drag 시작점)을 구한다.
        _startParent = transform.GetChild(0).GetChild(0);

        //Drag시 아이템 갯수가 표시 되지 않게함
        quantityText.enabled = false;

        //Drag시 sort에의해 Drag중인 아이템이 가려지는 현상 방지.
        //ItemSlot은 InventoryCanvas 안에 그려지고 있다.
        //Drag 시 itemImage의 위치를 InventoryCanvas 의 Dragtemp밑으로 옮겨주어 sort를 높여 이미지가 잘리지 않도록 한다.       
        itemImage.transform.SetParent(Dragtemp.transform); // Dragtemp = InventoryCanvas의 마지막에 위치한 Object로 sort가 가장 높다.
                                                           //InventoryCanvas의 상위 폴더인 UIManager로 빼게 되면 UIManager는 Canvas가 아니기 때문에 sort가 맞지 않아 이미지가 보이지 않게 된다.


        //sort가 가장위로 올라오면 뒤에 있는 오브젝트를 막기 때문에 DropSlot를 찾을 수 없게 된다.
        //이미지의 레이를 꺼주어 Drop될 오브젝트를 판별 가능하게 한다. 
        itemImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }
        //아이템 이미지가 이동하는걸 보여준다.
        itemImage.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        isDrag = false;
        //Drag시 표시되지 않게 했던 아이템 갯수 표시 시켜줌
        quantityText.enabled = true;

        //드래그가 끝나는 지점의 오브젝트들을 hoveredObject에 저장
        List<GameObject> hoveredObject = eventData.hovered;

        //EventSystem.current.IsPointerOverGameObject() : 오브젝트가 UI와 겹치면 false 그렇지 않으면 true
        //false 일 시 UI가 아닌 게임상 오브젝트 임을 명시
        //Drag의 끝이 UI가 아닐 경우 아이템을 버린다.
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            DropItem();
        }
        else
        {
            for (int i = 0; i < hoveredObject.Count; i++)
            {
                //hoveredObject에서 자신이 아닌 Slot 오브젝트를 찾는다. 
                if (hoveredObject[i] != gameObject && hoveredObject[i].CompareTag("Slot"))
                {
                    //Debug.Log(hoveredObject[i]);
                    ItemSlot dropSlot = hoveredObject[i].GetComponent<ItemSlot>();
                    //Drag슬롯의 이미지와 아이템(temp 역할)
                    Image dragItemImage = itemImage;
                    TextMeshProUGUI dragQuantityText = quantityText;
                    Item dragItem = item;
                    //Drag슬롯에 있는 아이템의 갯수
                    int dragquntity = quantity;

                    Inventory.InvenItem _dragItem = new Inventory.InvenItem(inven.invenItems[slotNum].slotItem, inven.invenItems[slotNum].quantity,true);


                    //포션 아이템의 잔량을 합치는 기능
                    //상황> 최대소지가 20개인 포션이 A와 B슬롯에 있을 때 B슬롯을 Drag하여 A슬롯에 Drop 했을 시.
                    //예) A슬롯에 6개, B슬롯에 7개가 있을 때 A슬롯은 13개가 되고 B슬롯은 빈 슬롯이 되어야 한다.
                    //예2) A슬롯에 19개, B슬롯에 3개 있을 때 A슬롯은 20개가 되어야하고, B슬롯은 2개가 되어야 한다.
                    //아이템이 포션이고, Drop슬롯에 아이템이 존재하며, A슬롯과 B슬롯 모두 아이템 중첩이 가능할 때 위 조건이 실행가능하다. + 같은 종류의 포션이여야 한다.
                    if (item.itemType == ItemType.Potion && dropSlot.item != null && isOverlap == true && dropSlot.isOverlap == true && item.itemNumber == dropSlot.item.itemNumber)
                    {
                        //같은 종류의 포션일 때
                        if (item.itemName == dropSlot.item.itemName)
                        {
                            //Drag슬롯과 Drop슬롯의 갯수가 최대 소지 한도보다 작거나 같다면
                            if ((quantity + dropSlot.quantity) <= (dropSlot.item as Potion).ownershipLimit)
                            {
                                //Drop 슬롯 갯수 증가
                                dropSlot.quantity = quantity + dropSlot.quantity;
                                //Drag 슬롯 초기화
                                quantity = 0;
                                RemoveItemSlot();
                                inven.RemoveItem(slotNum);
                            }
                            //Drag슬롯과 Drop슬롯의 갯수가 최대 소지 한도보다 많다면
                            else
                            {
                                //Drop 슬롯에 넘겨주고 남은 갯수만 남김
                                quantity = quantity - ((dropSlot.item as Potion).ownershipLimit - dropSlot.quantity);

                                //Drop 슬롯은 최대치로 채움
                                dropSlot.quantity = (dropSlot.item as Potion).ownershipLimit;
                            }
                            QuantityItem();
                            dropSlot.QuantityItem();
                            CheckUseSlot();
                            dropSlot.CheckUseSlot();
                        }
                    }
                    else
                    {
                        //Drop슬롯의 아이템 이미지를 Drag슬롯에 넣어준다. (위치 교환)
                        dropSlot.itemImage.transform.SetParent(_startParent);
                        //이미지의 위치 보정
                        dropSlot.itemImage.transform.localPosition = Vector3.zero;

                        //Drag슬롯의 아이템 이미지를 Drop된 슬롯에 넣어준다. (위치 교환)
                        itemImage.transform.SetParent(hoveredObject[i].transform.GetChild(0).GetChild(0));
                        //이미지의 위치 보정
                        itemImage.transform.localPosition = Vector3.zero;

                        //Drag시작시 꺼두었던 레이를 켜주어 다시 드래그시 감지 할 수 있도록 한다.
                        itemImage.raycastTarget = true;
                        dropSlot.itemImage.raycastTarget = true;

                        //Drag 슬롯을 선택하면 Drop슬롯의 아이템을 받아 오기 때문에 item도 변경한다.
                        //Drop슬롯의 아이템을 받아온다. (아이템 교환)
                        item = dropSlot.item;
                        //Drop슬롯의 아이템 이미지를 받아온다. (이미지 교환)
                        itemImage = dropSlot.itemImage;
                        
                        //Drop슬롯에 있는 아이템의 갯수를 받아온다. (갯수 교환)
                        quantity = dropSlot.quantity;
                        QuantityItem();

                        //Drag 슬롯에 저장된 item을 변경하지 않으면 슬롯에 저장된 아이템은 Drop슬롯에 Drag 아이템이 존재하게 된다
                        //Drop슬롯 오브젝트에 Drag슬롯의 오브젝트를를 넣어준다.
                        dropSlot.itemImage = dragItemImage;
                        dropSlot.item = _dragItem.slotItem;
                        dropSlot.quantity = dragquntity;
                        dropSlot.QuantityItem();
                        //dropSlot.inven.invenItems[dropSlot.slotNum] = _dragItem;

                        //swap 완료된 슬롯의 아이템을 Inventory에 넘겨준다.
                        inven.items[slotNum] = item;
                        inven.items[dropSlot.slotNum] = dropSlot.item;

                        //인벤토리 사용가능 및 포화여부 체크               
                        CheckUseSlot();
                        dropSlot.CheckUseSlot();
                        AudioManager.instance.PlayeSound("dragEnd");
                    }
                }
                else if (hoveredObject[i].CompareTag("Equipslot") && item.itemType == ItemType.Equip)
                {

                    if (hoveredObject[i].GetComponent<EquipItemSlot>().slotTypeNum == (int)(((EquipItem)item).equipType))
                    {
                        Debug.Log("아이템 장착");
                        UseItem();
                    }
                }
            }
        }

        itemImage.raycastTarget = true;
        itemImage.transform.SetParent(_startParent);
        itemImage.transform.localPosition = Vector3.zero;

        //GetComponentInParent<UICheck>().enabled = false;
        //GetComponentInParent<UICheck>().enabled = true;

        invenUI.DeSelectAllSlots();
        //리스트 정리를 해주지 않으면 hoveredObject가 계속 쌓여 버그 발생
        
        hoveredObject.Clear();
    }

    //아이템 드래그를 유지하면서 인벤토리를 닫았을 시 이미지가 원래 위치로 돌아가지 않고 
    //인벤토리가 닫힐 때 있던 위치에 이미지가 그대로 남는 현상을 방지함
    public void OnDisable()
    {
        if (isDrag)
        {
            itemImage.transform.SetParent(_startParent);
            itemImage.transform.localPosition = Vector3.zero;
            QuantityItem();
            //isDrag = false; //김현수 지분
        }
    }

}
