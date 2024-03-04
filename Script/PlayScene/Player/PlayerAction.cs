using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAction : MonoBehaviour
{
    public PlayerController player;
    public GameObject weaponSlot;
    private int actionState;

    private AudioManager audioManager;
    public Sound[] playerSound;

    private void Start()
    {
        audioManager = AudioManager.instance;

        actionState = Animator.StringToHash("Base Layer.Footman_Action");

        foreach (var s in playerSound)
        {
            audioManager.AddExternalSound(s);
        }
    }

    private void Update()
    {       
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash == actionState)
        {
            //EndAction();
        }
    }

    public void EndAction()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85)
        {
            PlayerController.isAction = false;
            player.SetState(PlayerState.Idle);
        }
    }

    public void CheckAttack()
    {
        weaponSlot.GetComponent<Weapon>().SetAttackDamage();
    }

    public void CheckAction()
    {
        PlayerController.isAction = false;
        player.SetState(PlayerState.Idle);
        Debug.Log("??");
    }

    public void StartAttack()
    {   
        weaponSlot.GetComponent<MeshCollider>().enabled = true;
    }

    public void EndAttack()
    {
        weaponSlot.GetComponent<MeshCollider>().enabled = false;
        weaponSlot.GetComponent<Weapon>().isMultiple = false;
        weaponSlot.GetComponent<Weapon>().enemies.Clear();
    }

    public void SetCastStay()
    {
        GetComponent<Animator>().SetFloat("MotionProccess", 0.5f);
    }

    public void ResetCastMotion()
    {
        GetComponent<Animator>().SetFloat("MotionProccess", 0f);
    }

    public void Replay(float _point)
    {
        GetComponent<Animator>().Play("Footman_Action", -1, _point);
    }

    public void CheckNomalized()
    {
        Debug.Log(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public void PlayerSound(string _playerSound)
    {
        audioManager.PlayExSound(_playerSound);
    }
}
