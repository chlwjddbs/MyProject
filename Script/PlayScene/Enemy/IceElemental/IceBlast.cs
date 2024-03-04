using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlast : MonoBehaviour
{
    public GameObject iceBlast;
    private float AttackDamage = 50000;
    private Vector3 blastRange = new Vector3(1f,1f,1f);
    private GameObject iceElemental;
    public ParticleSystem magicCircle;
    public ParticleSystem light_1;
    public ParticleSystem light_2;

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {  
        iceElemental = GameObject.Find("IceElemental");
        iceElemental.GetComponent<IceElemental>().blast = BlastGimmick;
    }

    private void Update()
    {
        if (iceElemental.GetComponent<IceElemental>().isDeath)
        {
            magicCircle.Stop();
            light_1.Stop();
            light_2.Stop();
            stopSound();
        }
    }

    public void BlastGimmick()
    {
        if (transform.localScale.x > 5)
        {
            transform.localScale -= blastRange;
        }
    }

    public void stopSound()
    {
        audioSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatus>().TakeDamage(AttackDamage);
            Debug.Log("blast");
        }
    }
}
