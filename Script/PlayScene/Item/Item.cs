using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    //아이템 고유 번호
    public int itemNumber;

    //아이템 타입(장비, 포션, 퍼즐 등)
    public ItemType itemType;

    //아이템 이름
    public string itemName;

    //아이템 설명
    public string description;

    //아이템 무게
    public float itemWeight;
    
    //아이템 이미지
    public Sprite itemImege;

    //아이템 상점 매입가
    public int shopPrice;

    //아이템 상점 매각가
    public int sellPrice;


    //필드에서 보이는 아이템 오브젝트
    public GameObject FieldObject;

    public virtual void Use(int slotNum)
    {
        Debug.Log("소모품 사용");
    }
    
}

public enum ItemType
{
    Equip,          //장비   
    Ingredient,     //재료
    Potion,         //포션
    Puzzle,         //퍼즐
    SkillBook,      //스킬북
}