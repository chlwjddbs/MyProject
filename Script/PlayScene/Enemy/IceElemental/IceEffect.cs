using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEffect : MonoBehaviour
{
    public IceElemental olf;
    private PlayerStatus playerStatus;
    private PlayerController player;
    public float slowRate = 0.4f; 
    private float slowTime = 2f;
    private float count;

    private bool isStay = false;

    // Update is called once per frame
    void Update()
    {
        count = Mathf.Clamp(count, 0, slowTime);

        if(count <= 0)
        {
            if(playerStatus != null)
            {
                isStay = false;
                playerStatus.ResetMoveSpeed();
            }
        }
        count -= Time.deltaTime;
        //Debug.Log(count);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isStay)
            {
                isStay = true;
                olf.PlayEnemySound("freeze");
            }
            player = other.GetComponent<PlayerController>();
            playerStatus = other.GetComponent<PlayerStatus>();
            playerStatus.moveSpeed = playerStatus.baseMoveSpeed * (1 - slowRate);
            playerStatus.moveSpeedUI.text = playerStatus.moveSpeed.ToString();
            count = slowTime;
        }
    }

    private void OnDisable()
    {
        if (playerStatus != null)
            playerStatus.ResetMoveSpeed();
    }
}
