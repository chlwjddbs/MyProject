using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElementalTrigger : MonoBehaviour
{
    public Animator FireRoad;
    public ParticleSystem SpawnFireElementalEffect;
    public ParticleSystem FireAura;
    public FireElemental FireElemental;
    public SetCursorImage cursorAction;

    public List<BoxCollider> gate;

    public GameObject Obstacle;

    private bool isDeath = false;

    public Sound[] sound;

    private void Start()
    {
        foreach (var s in sound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    private void Update()
    {     
        if (FireElemental.isDeath && !isDeath)
        {
            isDeath = true;
            FireRoad.SetBool("Offtrigger", true);
            Obstacle.SetActive(false);
            OpenTheGate();
        }

        if (!FireElemental.isDeath)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == "SpawnTrigger")
                    {
                        if (cursorAction.theDistance < cursorAction.actionDis)
                        {
                            FireElemental.FireAura.SetActive(true);
                            Destroy(cursorAction.gameObject, .3f);
                            StartCoroutine("SpawnFireElemental");
                        }
                    }

                }
            }
        }
        /*
        if (FireElemental.GetComponent<FireElemental>().isPase_2)
        {

        }
        */
    }

    IEnumerator SpawnFireElemental()
    {
        AudioManager.instance.StopBGM(1.5f);
        Obstacle.SetActive(true);
        FireRoad.SetBool("Ontrigger", true);
        LockTheGate();
        SpawnFireElementalEffect.Play();
        AudioManager.instance.PlayExSound("gn_awake");

        yield return new WaitForSeconds(1f);

        AudioManager.instance.PlayBGM("Fight_Gubne",1f);
        FireElemental.gameObject.SetActive(true); 
        FireAura.Play();
        yield return new WaitForSeconds(1f);

        SpawnFireElementalEffect.Stop();

        yield return new WaitForSeconds(1f);
        Destroy(SpawnFireElementalEffect);

        FireElemental.isActive = true;
        FireElemental.GetComponent<CapsuleCollider>().enabled = true;
    }

    void LockTheGate()
    {
        for (int i = 0; i < gate.Count; i++)
        {
            gate[i].isTrigger = true;
        }
    }

    void OpenTheGate()
    {
        for (int i = 0; i < gate.Count; i++)
        {
            gate[i].isTrigger = false;
        }
    }

}
