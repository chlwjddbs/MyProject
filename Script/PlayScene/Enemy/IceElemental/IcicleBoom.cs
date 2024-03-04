using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleBoom : MonoBehaviour
{
    private float countdown = 1.5f;
    public float attackDamage = 10f;
    private GameObject iceElemental;

    private float iceBlastGaugePoint = 10f;

    // Start is called before the first frame update
    void Start()
    {
        iceElemental = GameObject.Find("IceElemental");
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0)
        {
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Slowly");
            AudioManager.instance.PlayExSound("iceBoom");
            GetComponent<SphereCollider>().enabled = false;
            other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);
            iceElemental.GetComponent<IceElementalAction>().AddBlastGauge(iceBlastGaugePoint);
        }
    }
}
