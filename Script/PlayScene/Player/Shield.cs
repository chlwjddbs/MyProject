using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public EquipItem equipShield;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GetComponentInParent<PlayerStatus>();
    }

    //아이템 장착시
    public void EquipShield(EquipItem _shield)
    {
        equipShield = _shield;

        //장착한 아이템의 공격력 추가
        //playerStatus.equipDefence = equipShield.defence;
    }

    //아이템 장착 해제시
    public void UnequipShield()
    {
        //무기 공격력 제거
        //playerStatus.equipDefence -= equipShield.defence;
        equipShield = null;
    }

}
