using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject playerStatusUI;
    public PlayerStatus playerStatus;
    public Image equipWeaponImage;
    public Sprite UnequipWeaponImage;
    public Image HpBar;
    public Image MpBar;
    // Start is called before the first frame update
    void Start()
    {
        //게임 시작시 무기를 장착하지 않은 빈슬롯 이미지를 보여준다.
        equipWeaponImage.sprite = UnequipWeaponImage;
    }

    //플레이어의 HP에 변동이 생겼을 시 hpbar fillamount 변경
    public void SetHpBar()
    {
        HpBar.fillAmount = playerStatus.remainHealth / playerStatus.maxHealth;
        playerStatus.hpUI.text = playerStatus.remainHealth + " / " + playerStatus.maxHealth;
        PlayerStatus.stausUI?.Invoke();
    }

    //플레이어의 MP에 변동이 생겼을 시 mpbar fillamount 변경
    public void SetMpBar()
    {
        MpBar.fillAmount = playerStatus.remainMana / playerStatus.maxMana;
        playerStatus.mpUI.text = playerStatus.remainMana + " / " + playerStatus.maxMana;
        PlayerStatus.stausUI?.Invoke();
    }

    //장착한 장비의 이미지를 보여준다.
    public void EquipWeapon(Sprite _weaponImage)
    {
        equipWeaponImage.sprite = _weaponImage;
    }

    //빈 슬롯 이미지를 보여준다.
    public void UnEquipWeapon()
    {
        equipWeaponImage.sprite = UnequipWeaponImage;
    }
}
