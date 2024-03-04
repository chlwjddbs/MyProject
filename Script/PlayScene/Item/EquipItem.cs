using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equip", menuName = "Equip")]
public class EquipItem : Item
{
    //장비의 타입 (무기, 방패, 갑옷 등)
    public EquipType equipType;

    //장비 공격력
    public float attack;

    //장비 방어력
    public float defence;

    //장비 체력
    public float health;

    //장비 마나
    public float mana;

    //실제 장착할 장비(인게임에 표시될 장비 오브젝트)
    public GameObject equipItemObject;

    //실제 장착한 장비의 위치 오프셋( 인게임에 표시되는 장비의 위치)
    public Vector3 offset;

    //공격 시 타격 판정에 필요한 콜라이더를 mesh로 받아온다.
    public Mesh mesh;

    //무기에 따른 기본 공격 애니메이션
    public AnimationClip attackClip;

    public override void Use(int _slotNum)
    {
        AudioManager.instance.PlayeSound(equipType.ToString() + "Sound");
        Equipment.instance.EquipItem(this, _slotNum);
    }
}

public enum EquipType
{
    Head,
    Weapon,
    Shield,
    Armor,
    Shoes,
    Ring,

    EquipTypeMax,
}
