using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Posion",menuName = "Posion/Hp_Posion")]
public class HpPotion : Potion
{
    public float recoveryPoint;

    public override void Use(int slotNum)
    {
        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        player.RecoveryHP(recoveryPoint);
    }
}
