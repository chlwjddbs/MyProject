using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Posion", menuName = "Posion")]
public class Potion : Item
{
    public PotionTpye potionTpye;

    //포션 최대소지 한도
    public int ownershipLimit;

    //현재 소지 개수
    public int quntity = 0;

    //public GameObject potionItemObject;

}

public enum PotionTpye
{
    Hp,         //HP 회복 포션
    Mp,         //MP 회복 포션
    Detoxion,   //상태이상 해제 포션
    Buff,       //버프 포션
}