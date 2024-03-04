using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Equipment : MonoBehaviour
{
    public static Equipment instance;
    //장비 장착칸은 장착 할 수 있는 파츠의 수이다.
    //예를 들어 무기, 방어구 , 신발을 장착 가능하다면 장비 장착칸은 3일 것이다.
    public int equipmentSize = (int)EquipType.EquipTypeMax;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public UnityAction<EquipItem> AddUpdateEquip;

    public UnityAction<EquipItem> RemoveUpdateEquip;

    public UnityAction UpdateDamage;

    //장비 장착칸 배열로 생성
    public EquipItem[] equipItems;

    private Inventory inven;

    public Weapon weapon;
    public Shield shield;
    public PlayerStatus playerStatus;

    public UnityAction SetEquipData;

    
    public void SetData()
    {
        if(instance == null)
        {
            Debug.Log("장착창 싱글톤 생성 대기중");
            SetData();
            return;
        }

        if(SetEquipData == null)
        {
            SetData();
            return;
        }

        inven = Inventory.instance;
        //배열의 크기를 장착할 수 있는 장비 파츠의 수로 지정한다.
        equipItems = new EquipItem[equipmentSize];

        SetEquipData?.Invoke();

        if (DataManager.instance.newGame)
        {

        }
        else
        {
            LoadData();
        }
    }

    //EquipItem으로부터 장착할 아이템을 받아온 후 장착한 아이템 표시
    public void EquipItem(EquipItem newEquipItem, int slotNum)
    {
        //새로 장착할 아이템 타입
        int newEquipType = (int)newEquipItem.equipType;

        //현재 장착중인 아이템이 있다면
        if (equipItems[newEquipType] != null)
        {
            //현재 장착중인 아이템을 인벤토리로 넘겨준 뒤 
            //inven.AddItem(equipItems[newEquipTyep]);
            inven.SwapItem(equipItems[newEquipType], slotNum);
            //장착중인 장비 탈착
            UnEquipItem(newEquipType);
        }

        //현재 장비 슬롯에 새로 들어온 아이템 장착
        equipItems[newEquipType] = newEquipItem;
        AddUpdateEquip?.Invoke(newEquipItem);

        //장착한 아이템이 무기 일 경우 
        if(newEquipItem.equipType == EquipType.Weapon)
        {
            //무기 정보를 넘겨준다.
            weapon.EquipWeapon(newEquipItem);
        }
        //장착한 아이템이 방패 일 경우
        else if(newEquipItem.equipType == EquipType.Shield)
        {
            //shield.EquipShield(newEquipItem);
        }

        //playerStatus.equipDamage += newEquipItem.attack;
        //playerStatus.equipDefence += newEquipItem.defence;
        //playerStatus.CurrentStatus();
        playerStatus.Equip(newEquipType, equipItems[newEquipType]);
        //UpdateDamage 이벤트를 통한 무기 정보 업데이트는 Equip으로 통합됌.
        //UpdateDamage?.Invoke();
    }

    public void UnEquipItem(int _slotNum)
    {
        RemoveUpdateEquip?.Invoke(equipItems[_slotNum]);
        if(equipItems[_slotNum].equipType == EquipType.Weapon)
        {
            weapon.UnequipWeapon();
        }
        //playerStatus.equipDamage -= equipItems[_slotNum].attack;
        //playerStatus.equipDefence -= equipItems[_slotNum].defence;

        //장비의 스텟을 빼주어야 하기 때문에 기본 값인 0을 받을 수 있도록 장비 정보는 넘겨주지 않는다.
        playerStatus.Unequip(_slotNum, equipItems[_slotNum]);
        equipItems[_slotNum] = null;
        //UpdateDamage?.Invoke();
    }
    
    public void SetEquip()
    {
        for (int i = 0; i < equipItems.Length; i++)
        {
            if(equipItems[i] != null)
            {
                AddUpdateEquip?.Invoke(equipItems[i]);

                //장착한 아이템이 무기 일 경우 
                if (equipItems[i].equipType == EquipType.Weapon)
                {
                    //무기 정보를 넘겨준다.
                    //인게임 내 무기 mesh 적용
                    weapon.EquipWeapon(equipItems[i]);
                }
                //장착한 아이템이 방패 일 경우
                else if (equipItems[i].equipType == EquipType.Shield)
                {
                    //shield.EquipShield(newEquipItem);
                }

                //playerStatus.equipDamage += equipItems[i].attack;
                //playerStatus.equipDefence += equipItems[i].defence;
                playerStatus.Equip(i, equipItems[i]);
            }
        }
    }

    public void SaveData()
    {
        DataManager.instance.userData.equipmentItem = equipItems;
    }

    public void LoadData()
    {
        equipItems = DataManager.instance.userData.equipmentItem;
        SetEquip();
    }
}
