using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InvenItem
    {
        public Item slotItem;
        public int quantity;

        public InvenItem(Item _item, int _quantity = 0, bool _isTemp = false)
        {
            slotItem = _item;

            if (slotItem == null)
            {
                quantity = 0;
            }

            if (_isTemp)
            {
                quantity = _quantity;
            }
        }
    }


    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //인벤토리 업데이트 시 실행될 액션 함수 등록
    //public UnityAction<int, Item> UpdateInventory;
    public UnityAction<int> AddUpdateUI;

    public UnityAction<int> RemoveUpdateUI;

    //public List<Item> items = new List<Item>();
    public int invenSize;
    //아이템이 들어온 슬롯번호
    public int slotNum = -1;
    public Item[] items;
    //다량 아이템의 수량 배열
    //public int[] quantityCheck;

    //인벤토리 소모율
    private int spareSlot = 0;
    //인벤토리에 아이템을 추가 할 수 있나요?
    public bool isAdd = true;

    //중첩 가능한 아이템 획득 시 습득한 아이템과 같은 아이템이 있는 슬롯과 중첩 가능 공간이 남은 슬롯 체크
    public UnityAction<Item> CheckUseableSlot;
    public bool UseableSlot = false;

    //중첩 가능한 아이템이 중첩되어야 하는지, 중첩이 불가능 하여 새로운 슬롯을 차지해야 하는지 체크
    public UnityAction<int, Item> GoodsOverlap;
    public bool isOverlap = false;

    public UnityAction SetInvenData;

    public Dictionary<int, InvenItem> invenItems = new Dictionary<int, InvenItem>();

    /*
    private void Start()
    {
        if (DataManager.instance.newGame)
        {
            Debug.Log("new Game");
            //invenSize = 25;
            spareSlot = 0;
            items = new Item[invenSize];
            InventorySpace();
        }
    }
    */

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (KeyValuePair<int,InvenItem> item in invenItems)
            {
                Debug.Log($"{item.Key}번 슬롯 : {item.Value.slotItem.itemName}의 갯수 = {item.Value.quantity}개");
            }
        }
    }
    */

    public void SetData()
    {
        if(instance == null)
        {
            Debug.Log("인벤토리 싱글톤 생성 대기중");
            SetData();
            return;
        }

        if (SetInvenData == null)
        {
            SetData();
            Debug.Log("UI 데이터 준비중");
            return;
        }

        SetInvenData?.Invoke();

        if (DataManager.instance.newGame)
        {
            //Debug.Log("new Game");
            invenSize = 25;
            spareSlot = 0;
            items = new Item[invenSize];

            for (int i = 0; i < invenSize; i++)
            {
                invenItems.Add(i, new InvenItem(null));
            }
        }
        else
        {
            //LoadData();
        }

        InventorySpace();
    }

    public void LoadData()
    {
        invenSize = DataManager.instance.userData.invenSize;
        spareSlot = DataManager.instance.userData.spareSlot;
        items = (Item[])(DataManager.instance.userData.inventoryItem).Clone();

        for (int i = 0; i < invenSize; i++)
        {
            invenItems.Add(i, new InvenItem(DataManager.instance.userData.invenItem[i].slotItem));
            /*
            if(DataManager.instance.userData.slotNum.Contains(i))
            {
                Debug.Log(i);
                int test1= 0;
                for (int j = 0; j < DataManager.instance.userData.slotNum.Count; j++)
                {
                    if (DataManager.instance.userData.slotNum.Contains(i))
                    {
                        test1 = j;
                        break;
                    }    
                }
                invenItems.Add(i, DataManager.instance.userData.invenItem[test1]);
            }
            else
            {
                invenItems.Add(i, null);
            }
            */
        }

        /*
        for (int i = 0; i < invenSize; i++)
        {
            if (DataManager.instance.userData.invenItem != null) 
            {              
                invenItems.Add(i, DataManager.instance.userData.invenItem[i]);
            }
            else
            {
                invenItems.Add(i, null);
            }
        }
        */

        //Debug.Log(DataManager.instance.userData.invenItems[0].slotItem.itemName);
        //invenItems = new Dictionary<int, InvenItem>(DataManager.instance.userData.invenItems);

        /*
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                AddUpdateUI?.Invoke(i);
            }
        }
        */
    }
    public void SaveData()
    {
        DataManager.instance.userData.inventoryItem = items;
        DataManager.instance.userData.invenSize = invenSize;
        DataManager.instance.userData.spareSlot = spareSlot;

        for (int i = 0; i < invenSize; i++)
        {
            if (DataManager.instance.userData.slotNum.Contains(i))
            {
                DataManager.instance.userData.invenItem[i] = invenItems[i];
            }
            else
            {
                DataManager.instance.userData.slotNum.Add(i);
                DataManager.instance.userData.invenItem.Add(invenItems[i]);
            }
        }

        /*
        DataManager.instance.userData.inventoryItem = new Item[invenSize];
        for (int i = 0; i < invenSize; i++)
        {
            DataManager.instance.userData.inventoryItem[i] = items[i];
        }
        */
    }

    public void AddItem(Item _item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] == null)
            {
                items[i] = _item;
                slotNum = i;
                invenItems[slotNum].slotItem = _item;
                AddUpdateUI?.Invoke(slotNum);

                //아이템이 들어오면 인벤토리의 소모율 증가
                spareSlot += 1;
                InventorySpace();
                break;
            }
        }
        //items.Add(_item);
        //액션함수가 등록되어 있을 시 실행
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }

    //포션 아이템 습득시
    public void AddPotion(Item _potion)
    {
        //습득한 아이템이랑 동일한 템이 있고, 중첩 가능한 여유 공간이 남은 슬롯 체크
        CheckUseableSlot?.Invoke(_potion);
        //Debug.Log(UseableSlot);
        //동일한 템이 있다면
        if (UseableSlot)
        {
            for (int i = 0; i < items.Length; i++)
            {
                //아이템이 없는 슬롯은 패스
                if(items[i] == null)
                {
                    continue;
                }

                //같은 아이템이 있는 슬롯 찾기
                else if (items[i].itemName == _potion.itemName)
                {
                    //새로 습득한 아이템이 현재 슬롯에 중첩 가능한지 체크
                    GoodsOverlap?.Invoke(i,_potion);

                    //가능하면 아이템 중첩
                    if (isOverlap)
                    {
                        AddUpdateUI?.Invoke(i);
                        //Debug.Log("중첩");
                        break;
                    }

                    //불가능 중첩 가능한 다음 슬롯을 찾는다.
                    //전체 슬롯에서 중첩 가능한 슬롯은 존재 할수 없다.
                    //CheckUseableSlot?.Invoke(_potion.itemName); 으로 중첩 가능한 공간이 남은 슬롯을 판정했기 때문.
                }
            }
        }
        //동일한 템이 없거나, 동일한 템이 존재하여도 중첩가능 공간이 남아 있지 않았다면
        else
        {
            //빈 슬롯에 아이템 추가
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = _potion;
                    slotNum = i;
                    invenItems[slotNum].slotItem = _potion;
                    AddUpdateUI?.Invoke(slotNum);

                    //아이템이 들어오면 인벤토리의 소모율 증가
                    spareSlot += 1;
                    InventorySpace();

                    Debug.Log("아이템 추가");
                    break;
                }
            }
        }

        //사용 가능한 인벤토리는 반듯이 존재한다. 왜냐하면 아이템을 인벤토리에 등록하기전, 습득단계에서 획득 가능 여부를 체크하기 때문
    }


    public void SwapItem(Item _item, int _slotNum)
    {
        items[_slotNum] = _item;
        slotNum = _slotNum;
        AddUpdateUI?.Invoke(slotNum);
    }

    public void RemoveItem(int _slotNum)
    {
        RemoveUpdateUI?.Invoke(_slotNum);
        items[_slotNum] = null;
        invenItems[_slotNum].slotItem = null;

        slotNum = -1;

        //아이템을 제거하면 인벤토리 소모율 감소
        spareSlot -= 1;
        InventorySpace();
        //items.Remove(_item);
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }
    
    public void InventorySpace()
    {
        //인벤토리가 invenSize(인벤토리 한도)에 도달했는지 체크
        if(spareSlot == invenSize)
        {
            isAdd = false;
        }
        else
        {
            isAdd = true;
        }
    }

    /*
    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < invenSize; i++)
        {
            if(items[i].itemNumber == _item.itemNumber)
            {
                items[i] = null;
                slotNum = i;
                RemoveUpdateInventory?.Invoke(slotNum, items[slotNum]);
                break;
            }
        }
        //items.Remove(_item);
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }
    */
}

