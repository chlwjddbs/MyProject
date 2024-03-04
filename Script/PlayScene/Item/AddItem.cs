using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;

public class AddItem : Interaction
{
    //실제 아이템 정보
    public Item item;

    //필드에 보여지는 아이템 오브젝트
    public GameObject itemObject;

    //필드에 보이는 아이템 오브젝트 이름 UI
    public GameObject BackGroundUI;

    //필드에 보이는 아이템 이름이 적혀지는 TextMeshPro
    public TextMeshProUGUI itemnameText;

    public GameObject ItemShapeBox;

    private SetCursorImage cursor;

    //아이템의 수량
    public int quantity;

    private void Start()
    {
        BackGroundUI.SetActive(false);
        cursor = transform.GetComponent<SetCursorImage>();
    }

    public override void Update()
    {
        base.Update();
        DrawItem();
    }

    private void DrawItem()
    {
        if (cursor.isDraw && ItemShapeBox.activeSelf == false)
        {
            ItemShapeBox.SetActive(true);
        }
        else if(!cursor.isDraw && ItemShapeBox.activeSelf == true)
        {
            ItemShapeBox.SetActive(false);
        }
    }

    public override void OnMouseOver()
    {
        if (cursor.isDraw)
        {
            if (PlayerController.isUI | PlayerController.isAction)
            {
                DontAction();
                return;
            }
            else
            {
                if (BackGroundUI.activeSelf == false)
                {
                    SetItemName();
                    BackGroundUI.SetActive(true);
                }
                DoAction();
            }
        }
    }

    public override void DoAction()
    {
        player.isObject = true;
        if (Input.GetMouseButtonDown(0))
        {
            if (theDistance < actionDis)
            {
                //인벤토리가 가득찬 상태이지만 중첩 가능한 소모품일 경우 갯수 추가는 가능하기 때문에 체크한다.
                if (Inventory.instance.isAdd == false)
                {
                    //아이템이 슬롯에 들어갈 수 있는지 슬롯 사용 가능 여부 확인.
                    Inventory.instance.CheckUseableSlot?.Invoke(item);

                    //가능하다면 갯수 중첩
                    if (Inventory.instance.UseableSlot)
                    {
                        AddPotion();
                    }

                    //불가능하면 아이템 획득 불가
                    else
                    {
                        //인벤토리가 가득찼다는 안내 메시지 출력
                        Debug.Log("인벤토리가 가득 찼습니다.");
                        return;
                    }
                }

                //인벤토리가 가득차지 않았다면 아이템 타입에 따라 아이템 획득
                if (item.itemType == ItemType.Potion)
                {
                    AudioManager.instance.PlayeSound("getPotion");
                    AddPotion();
                }
                else
                {
                    /*
                    switch (item.itemType)
                    {
                        case ItemType.Equip:
                            AudioManager.instance.PlayeSound("addEquip");
                            break;
                        case ItemType.Ingredient:
                            break;
                        case ItemType.Potion:
                            break;
                        case ItemType.Puzzle:
                            break;
                        case ItemType.SkillBook:
                            break;
                        default:
                            break;
                    }
                    */

                    AudioManager.instance.PlayeSound("getItem");
                    Inventory.instance.AddItem(item);
                    Debug.Log(item.name + " : 아이템 획득");
                    player.SetState(PlayerState.Idle);
                    Destroy(itemObject);
                    //player.isItem = false;
                }

                //Cursor.SetCursor(CursorManager.instance.orginCursor, CursorManager.instance.hotSpot, CursorMode.Auto);
                //CursorManager.instance.ResetCursor();
            }
        }
    }

    //중첩 가능한 아이템의 경우 최대중첩 갯수 한도가 정해져 있기 때문에 for문을 통해 아이템을 하나씩 획득하여 소지 한도만큼한 획득 할 수 있게 한다.
    public void AddPotion()
    {
        //현재 아이템의 수량을 체크한다.
        int startQuantity = quantity;

        //아이템의 수량만큼 for문을 통해 아이템 획득 절차를 밟는다.
        for (int i = 0; i < startQuantity; i++)
        {
            //획득할 아이템의 정보를 넘겨주어 사용 가능한 슬롯을 찾는다.
            Inventory.instance.CheckUseableSlot?.Invoke(item);

            //인벤토리가 가득찼지만 사용 가능한 슬롯(아이템 갯수를 합칠 슬롯)이 있다면
            if (!Inventory.instance.isAdd && Inventory.instance.UseableSlot)
            {
                //아이템 추가
                Inventory.instance.AddPotion(item);
                //Inventory.instance.
                player.SetState(PlayerState.Idle);

                //아이템이 추가되었기 때문에 현재 남은 수량 감소
                quantity -= 1;

                //수량이 감소됨에 따라 화면에 보여지는 아이템 이름(갯수표시) 변경
                SetItemName();

                //아이템이 전부 획득되었을 시 오브젝트를 삭제하여 빈 오브젝트가 막는것을 방지
                if (quantity <= 0)
                {
                    Destroy(itemObject);
                }
            }
            else if (Inventory.instance.isAdd)
            {
                Inventory.instance.AddPotion(item);
                //Debug.Log(item.name + " : 아이템 획득");
                player.SetState(PlayerState.Idle);
                quantity -= 1;
                SetItemName();

                if (quantity <= 0)
                {
                    Destroy(itemObject);
                    //player.isItem = false;
                }
            }
            else
            {
                break;
            }
        }
        //CursorManager.instance.ResetCursor();
    }

    public override void DontAction()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
        if (BackGroundUI.activeSelf == true)
        {
            BackGroundUI.SetActive(false);
        }
    }

    public void SetItemName()
    {
        if (item.itemType == ItemType.Potion && quantity > 1)
        {
            itemnameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", item.itemName, LocalizationSettings.SelectedLocale) + " x " + quantity;
        }
        else
        {
            itemnameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", item.itemName, LocalizationSettings.SelectedLocale);
        }
    }


    private void OnDestroy()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }
}
