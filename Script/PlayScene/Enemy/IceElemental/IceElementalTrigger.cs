using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceElementalTrigger : MonoBehaviour
{
    public IceElemental IceElemental;
    public ParticleSystem Snow;
    //전투 시작시 플레이어를 못 돌아가게 막음
    public GameObject obstacleWall;
    private bool isDeath = false;
  
    // Update is called once per frame
    void Update()
    {
        if(IceElemental.isDeath == true & !isDeath)
        {
            isDeath = true;
            Snow.Stop();
            AudioManager.instance.StopAm("Snow");
            obstacleWall.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            AudioManager.instance.StopBGM(1f);
            AudioManager.instance.PlayBGM("PlayScene_Floor_1");
        }

        if (IceElemental.target.GetComponent<PlayerStatus>().isDeath)
        {
            AudioManager.instance.StopAm("Snow");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IceElemental.isDeath)
        {
            return;
        }

        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ChangeBGM());
            obstacleWall.SetActive(true);
            IceElemental.IceEffect.SetActive(true);
            IceElemental.isActive = true;
            IceElemental.PlayEnemySound("encounterOlf");
            IceElemental.GetComponent<IceElementalAction>().enabled = true;
            Snow.Play();
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    
    IEnumerator ChangeBGM()
    {
        AudioManager.instance.StopBGM(1f);
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlayBGM("Fight_Olf", 1f);
        AudioManager.instance.PlayAmSond("Snow");
    }
}
